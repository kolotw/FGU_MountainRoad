using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    public GameObject playerCar; // 指向車輛的引用
    public RectTransform compassImage; // 指北針圖像的 RectTransform

    void Update()
    {
        // 根據車輛的方向調整指北針的旋轉
        float angle = playerCar.transform.eulerAngles.y;
        compassImage.rotation = Quaternion.Euler(0, 0, -angle);
    }
}