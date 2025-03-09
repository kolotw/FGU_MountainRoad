// 引用 KartGame.KartSystems 命名空間
using KartGame.KartSystems;
using UnityEngine;

public class buttonStartPoint : MonoBehaviour
{
    // 宣告 ArcadeKart 變量
    ArcadeKart kart;

    void Start()
    {
        // 嘗試獲取同一個 GameObject 上的 ArcadeKart 組件
        kart = GetComponent<ArcadeKart>();

        // 如果找不到 kart，打印錯誤信息
        if (kart == null)
        {
            Debug.LogError("找不到 ArcadeKart 組件，請檢查 GameObject 配置");
        }
    }

    // 雲起樓按鈕事件
    public void 雲()
    {
        if (kart != null)
        {
            kart.goWhere("/雲起樓起點");
        }
    }

    // 大楓橋按鈕事件
    public void 大()
    {
        if (kart != null)
        {
            kart.goWhere("/大楓橋起點");
        }
    }
    public void 懷()
    {
        if (kart != null)
        {
            kart.goWhere("/觀景台起點");
        }
    }
}
