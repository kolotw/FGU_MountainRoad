using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    private VehicleNavigation navigation;
    private VehicleController controller;

    void Start()
    {
        // 获取导航与物理控制组件
        navigation = GetComponent<VehicleNavigation>();
        controller = GetComponent<VehicleController>();
    }

    void Update()
    {
        // 如果当前存在目标 waypoint
        if (navigation.currentWaypoint != null)
        {
            // 计算从车身到目标 waypoint 的方向向量
            Vector3 direction = (navigation.currentWaypoint.transform.position - transform.position).normalized;

            // 计算需要转向的角度
            float steerAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

            // 调用物理控制的转向与驱动力函数
            controller.ApplySteering(steerAngle);
            controller.ApplyMotorTorque();  // 此处恢复 ApplyMotorTorque 的调用
        }
    }
}
