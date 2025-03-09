using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private KartControls controls;
    private float inputDir; // 單一軸的輸入值，用於控制左右旋轉
    private bool moveForward = false; // 用來追踪前進按鈕是否被按下
    private bool moveBackward = false; // 用來追踪減速按鈕是否被按下
    private float gas = 0f;
    public float rotationSpeed = 50f; // 旋轉速度
    public float moveForce = 1900000f; // 推進力，根據質量設定合適的值
    public float maxSpeed = 20f; // 最大速度
    public float brakeForce = 300000f; // 煞車力
    public float drag = 1.0f; // 當前阻力
    public float deceleration = 0.5f; // 自動減速力

    private bool resetPos = false;
    GameObject[] cone;
    float longDist = 30f;
    Vector3 target;

    private Rigidbody rb; // 用來進行物理運動的 Rigidbody

    private void Awake()
    {
        // 初始化 PlayerControls
        controls = new KartControls();

        // 獲取 Rigidbody 組件
        rb = GetComponent<Rigidbody>();

        // 設置剛體的質量
        rb.mass = 1500; // 設置質量為 1500 kg

        // 檢查 Rigidbody 參數是否正確
        rb.useGravity = true; // 確保使用重力
        rb.drag = drag; // 設置阻力
        rb.angularDrag = 1.5f; // 控制轉向慣性

        // 綁定方向輸入事件
        controls.KartActionMaps.Val_Direction.performed += ctx => inputDir = ctx.ReadValue<float>();
        controls.KartActionMaps.Val_Direction.canceled += ctx => inputDir = 0; // 停止輸入時，重置為 0

        //G29
        controls.KartActionMaps.G29_Fwd.performed += ctx => gas = ctx.ReadValue<float>();
        controls.KartActionMaps.G29_Fwd.canceled += ctx => gas = 0; // 停止輸入時，重置為 0

        // 監聽前進按鈕
        controls.KartActionMaps.Btn_Forward.performed += ctx => moveForward = true;
        controls.KartActionMaps.Btn_Forward.canceled += ctx => moveForward = false;

        // 監聽減速按鈕
        controls.KartActionMaps.Btn_Backward.performed += ctx => moveBackward = true;
        controls.KartActionMaps.Btn_Backward.canceled += ctx => moveBackward = false;

        //search cone
        controls.KartActionMaps.ResetPosition.performed += ctx => resetPos = true;
        controls.KartActionMaps.ResetPosition.canceled += ctx => resetPos = false;
    }

    private void OnEnable()
    {
        controls.Enable(); // 啟用輸入控制
    }

    private void OnDisable()
    {
        controls.Disable(); // 禁用輸入控制
    }

    private void FixedUpdate()
    {
        print("123" + gas);
        // 控制車輛旋轉
        Vector3 rotation = new Vector3(0, inputDir * rotationSpeed * Time.fixedDeltaTime, 0);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation)); // 使用 Rigidbody 進行旋轉

        if (resetPos)
        {
            searchCone();
        }
        else if (moveForward)
        {
            Accelerate();
        }
        else if (moveBackward)
        {
            Brake();
        }
        else
        {
            ApplyDrag();
        }


    }
    void searchCone()
    {
        print("search cone");
        float dist;
        GameObject[] cone = GameObject.FindGameObjectsWithTag("cone");
        if (cone != null)
        {
            foreach (GameObject go in cone)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist < longDist)
                {
                    longDist = dist;
                    target = go.transform.position;
                }
            }
            target.x = target.x + 1f;
            target.y = target.y + 3f;
            target.z = target.z + 1f;
            this.transform.position = target;
            longDist = 50f;
        }
    }
    // 加速邏輯
    private void Accelerate()
    {
        // 當前速度小於最大速度時才進行推動
        if (rb.velocity.magnitude < maxSpeed)
        {
            // 沿著角色的前方施加推力
            rb.AddForce(transform.forward * moveForce * Time.fixedDeltaTime, ForceMode.Force);
        }
    }

    // 煞車邏輯
    private void Brake()
    {
        // 當前速度大於0時才進行煞車
        if (rb.velocity.magnitude > 0)
        {
            rb.AddForce(-transform.forward * brakeForce * Time.fixedDeltaTime, ForceMode.Force);
        }
    }

    // 自然減速邏輯
    private void ApplyDrag()
    {
        // 當前速度大於0時進行減速
        if (rb.velocity.magnitude > 0)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }
    }
}
