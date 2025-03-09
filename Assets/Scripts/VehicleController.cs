using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    public float motorTorque = 1500.0f; // 車輛的驅動力
    public float maxSteeringAngle = 15.0f; // 最大轉向角度，減少轉彎劇烈度
    public float maxSpeed = 20.0f; // 最大車速
    public float slipForwardExtremumValue = 0.5f; // 前進方向的摩擦力
    public float slipSidewaysExtremumValue = 0.5f; // 側向摩擦力

    private Rigidbody rb;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 增加車輛質量以提升穩定性
        rb.mass = 1500f;

        // 調整車輛的重心位置
        rb.centerOfMass = new Vector3(0, -0.9f, 0);
    }

    void Update()
    {
        currentSpeed = rb.velocity.magnitude;

        // 動態控制車輛的轉向
        Vector3 direction = transform.forward; // 根据导航系统获得方向
        float steerAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        ApplySteering(steerAngle);

        // 應用動態驅動力
        ApplyMotorTorque();

        // 动态调整摩擦力，防止打滑
        AdjustFriction(rearLeftWheel, currentSpeed);
        AdjustFriction(rearRightWheel, currentSpeed);

        // 在減速或剎車時增加摩擦力
        AdjustFrictionOnBraking();
    }

    // 控制車輛的轉向，限制最大轉向角度
    public void ApplySteering(float steerAngle)
    {
        steerAngle = Mathf.Clamp(steerAngle, -maxSteeringAngle, maxSteeringAngle);
        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;
    }

    // 應用動態驅動力，根據車速減少推力，防止高速失控
    public void ApplyMotorTorque()
    {
        float adjustedTorque = motorTorque;

        // 如果車速超過 10，減少推力，防止打滑
        if (currentSpeed > 10.0f)
        {
            adjustedTorque *= 0.5f; // 減少推力，防止在高速時失控
        }

        // 檢查是否超速
        if (currentSpeed < maxSpeed)
        {
            rearLeftWheel.motorTorque = adjustedTorque;
            rearRightWheel.motorTorque = adjustedTorque;
        }
        else
        {
            rearLeftWheel.motorTorque = 0;
            rearRightWheel.motorTorque = 0;
        }
    }

    // 根據速度動態調整摩擦力
    public void AdjustFriction(WheelCollider wheel, float currentSpeed)
    {
        WheelFrictionCurve forwardFriction = wheel.forwardFriction;
        WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;

        // 動態調整摩擦力，速度越快摩擦力越強
        if (currentSpeed < 5.0f)
        {
            forwardFriction.extremumValue = Mathf.Lerp(1.0f, 2.5f, currentSpeed / 5.0f);
            sidewaysFriction.extremumValue = Mathf.Lerp(1.0f, 2.5f, currentSpeed / 5.0f);
        }
        else if (currentSpeed < 10.0f)
        {
            forwardFriction.extremumValue = Mathf.Lerp(2.5f, 5.0f, (currentSpeed - 5.0f) / 5.0f);
            sidewaysFriction.extremumValue = Mathf.Lerp(2.5f, 5.0f, (currentSpeed - 5.0f) / 5.0f);
        }
        else
        {
            forwardFriction.extremumValue = Mathf.Lerp(5.0f, 10.0f, (currentSpeed - 10.0f) / 10.0f);
            sidewaysFriction.extremumValue = Mathf.Lerp(5.0f, 10.0f, (currentSpeed - 10.0f) / 10.0f);
        }

        wheel.forwardFriction = forwardFriction;
        wheel.sidewaysFriction = sidewaysFriction;
    }

    // 剎車時增加摩擦力
    public void AdjustFrictionOnBraking()
    {
        float brakeInput = Input.GetAxis("Brake"); // 根據實際剎車輸入來動態調整

        if (brakeInput > 0.1f) // 檢查是否正在減速或剎車
        {
            AdjustFriction(frontLeftWheel, rb.velocity.magnitude);
            AdjustFriction(frontRightWheel, rb.velocity.magnitude);
            AdjustFriction(rearLeftWheel, rb.velocity.magnitude);
            AdjustFriction(rearRightWheel, rb.velocity.magnitude);
        }
    }
}
