using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    public GameObject playerCar; // ���V�������ޥ�
    public RectTransform compassImage; // ���_�w�Ϲ��� RectTransform

    void Update()
    {
        // �ھڨ�������V�վ���_�w������
        float angle = playerCar.transform.eulerAngles.y;
        compassImage.rotation = Quaternion.Euler(0, 0, -angle);
    }
}