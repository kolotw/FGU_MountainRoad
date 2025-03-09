using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // ���H���ؼСA�Ҧp����
    public Vector3 offset;   // �ṳ���۹��ؼЪ���m����

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
