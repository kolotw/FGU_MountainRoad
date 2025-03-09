using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 跟隨的目標，例如車輛
    public Vector3 offset;   // 攝像機相對於目標的位置偏移

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
