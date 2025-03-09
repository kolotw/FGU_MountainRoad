using UnityEngine;
using UnityEngine.AI;
using System.Linq; // 用於排序

public class AiMove : MonoBehaviour
{
    public WheelCollider 前左輪;
    public WheelCollider 前右輪;
    public WheelCollider 後左輪;
    public WheelCollider 後右輪;

    public float 最大速度 = 20.0f; // 最大車速
    public float 距離閾值 = 0.5f; // 到達 waypoint 的距離閾值
    public float 上坡推力倍數 = 2.0f; // 上坡時增加推力的倍數
    public float 坡度閾值 = 10.0f; // 認為是上坡的坡度臨界值
    public float 旋轉速度 = 100.0f; // 車輛旋轉的速度
    public float 當前速度 = 10.0f; // 設置初始速度
    public float 引擎扭力 = 1500.0f; // 車輛的驅動力

    private NavMeshAgent 導航代理; // NavMeshAgent 組件，用於路徑計算
    private Rigidbody 剛體; // 車輛的剛體
    private GameObject[] 路徑點; // 所有的路徑點
    public GameObject 當前路徑點; // 當前目標路徑點
    private int 當前路徑點索引 = 0; // 當前路徑點的索引

    void Start()
    {
        導航代理 = GetComponent<NavMeshAgent>();
        剛體 = GetComponent<Rigidbody>();

        // 禁用 NavMeshAgent 自動更新位置和旋轉
        導航代理.updatePosition = false;
        導航代理.updateRotation = false;

        // 查找所有帶有標籤 "wayPoints" 的遊戲對象
        路徑點 = GameObject.FindGameObjectsWithTag("wayPoints");

        // 根據距離進行排序，從最近到最遠
        路徑點 = 路徑點.OrderBy(點 => Vector3.Distance(transform.position, 點.transform.position)).ToArray();

        // 調整車輛的懸掛和摩擦力
        調整懸掛();
        調整摩擦力(前左輪);
        調整摩擦力(前右輪);
        調整摩擦力(後左輪);
        調整摩擦力(後右輪);

        // 調整車輛重心
        調整重心();

        // 設置第一個目標路徑點
        if (路徑點.Length > 0)
        {
            當前路徑點 = 路徑點[當前路徑點索引];
            前往下一個路徑點();
        }
    }

    void Update()
    {
        if (當前路徑點 != null)
        {
            // 確保車輛與地面保持接觸
            確保輪胎接地();

            // 獲取目標方向
            Vector3 目標方向 = 當前路徑點.transform.position - transform.position;
            目標方向.y = 0; // 確保只在水平面移動

            float 距離 = 目標方向.magnitude;

            // 如果車輛已經接近目標點
            if (距離 <= 距離閾值)
            {
                Destroy(當前路徑點);
                獲取下一個路徑點();
            }
            else
            {
                // 檢查是否在上坡，如果是，增加推力
                檢查坡度並調整推力();

                // 計算移動的方向和速度
                Vector3 方向 = 目標方向.normalized;
                float 移動速度 = Mathf.Min(當前速度 * Time.deltaTime, 距離); // 避免超過目標點

                // 使用剛體的 MovePosition 進行平滑移動
                剛體.MovePosition(transform.position + 方向 * 移動速度);

                // 保持車頭朝向前進方向
                面向路徑點();
            }
        }
        else
        {
            獲取下一個路徑點();
        }
    }

    // 強制車頭朝向當前目標路徑點
    void 面向路徑點()
    {
        if (當前路徑點 == null) return;

        Vector3 路徑點方向 = 當前路徑點.transform.position - transform.position;
        路徑點方向.y = 0; // 確保只在水平面旋轉

        // 計算目標旋轉
        Quaternion 目標旋轉 = Quaternion.LookRotation(路徑點方向);

        // 平滑旋轉
        transform.rotation = Quaternion.Slerp(transform.rotation, 目標旋轉, Time.deltaTime * 旋轉速度);
    }

    // 檢查車輛是否在上坡，如果在上坡則增加推力
    void 檢查坡度並調整推力()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1.0f))
        {
            // 計算坡度角度
            float 坡度角度 = Vector3.Angle(hit.normal, Vector3.up);

            // 如果坡度超過指定的閾值，則認為是上坡，增加推力
            if (坡度角度 > 坡度閾值)
            {
                Debug.Log("正在上坡，增加推力！");
                引擎扭力 *= 上坡推力倍數; // 增加推力
            }
        }

        // 應用引擎扭力到後輪
        應用引擎扭力();
    }

    // 應用引擎扭力到後輪
    void 應用引擎扭力()
    {
        // 應用推力到後輪
        後左輪.motorTorque = 引擎扭力;
        後右輪.motorTorque = 引擎扭力;
    }

    // 調整車輛的懸掛系統
    void 調整懸掛()
    {
        // 懸掛系統參數
        JointSpring 懸掛彈簧 = new JointSpring();
        懸掛彈簧.spring = 20000; // 懸掛剛度
        懸掛彈簧.damper = 4500;  // 懸掛阻尼
        懸掛彈簧.targetPosition = 0.5f;

        // 設定懸掛距離
        前左輪.suspensionDistance = 0.3f;
        前右輪.suspensionDistance = 0.3f;
        後左輪.suspensionDistance = 0.4f;
        後右輪.suspensionDistance = 0.4f;

        // 應用懸掛彈簧參數
        前左輪.suspensionSpring = 懸掛彈簧;
        前右輪.suspensionSpring = 懸掛彈簧;
        後左輪.suspensionSpring = 懸掛彈簧;
        後右輪.suspensionSpring = 懸掛彈簧;
    }

    // 調整輪胎摩擦力
    void 調整摩擦力(WheelCollider 車輪)
    {
        WheelFrictionCurve 前進摩擦 = 車輪.forwardFriction;
        前進摩擦.stiffness = 2.0f; // 增加抓地力

        WheelFrictionCurve 側向摩擦 = 車輪.sidewaysFriction;
        側向摩擦.stiffness = 2.0f; // 增加側向穩定性

        車輪.forwardFriction = 前進摩擦;
        車輪.sidewaysFriction = 側向摩擦;
    }

    // 調整車輛重心
    void 調整重心()
    {
        剛體.centerOfMass = new Vector3(0, -0.5f, 0); // 調整重心位置
    }

    // 確保車輛與地面接觸
    void 確保輪胎接地()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1.0f))
        {
            float 地面距離 = hit.distance;

            // 根據檢測的地面距離來調整車輛高度，避免輪子懸空
            if (地面距離 > 0.5f) // 距離過大時，進行高度調整
            {
                transform.position -= new Vector3(0, 地面距離 - 0.5f, 0); // 調整車輛位置
            }
        }
    }

    // 按順序選擇下一個路徑點
    void 獲取下一個路徑點()
    {
        當前路徑點索引++;
        if (當前路徑點索引 < 路徑點.Length)
        {
            當前路徑點 = 路徑點[當前路徑點索引];
            前往下一個路徑點();
        }
        else
        {
            Debug.Log("所有路徑點已經訪問完畢！");
            導航代理.isStopped = true; // 停止 NavMeshAgent
        }
    }

    // 設定車輛前往下一個路徑點
    void 前往下一個路徑點()
    {
        if (當前路徑點 != null)
        {
            NavMeshPath 計算出的路徑 = new NavMeshPath();
            導航代理.CalculatePath(當前路徑點.transform.position, 計算出的路徑);

            if (計算出的路徑.status == NavMeshPathStatus.PathComplete)
            {
                Debug.Log("前往路徑點: " + 當前路徑點.name);
            }
            else
            {
                Debug.LogWarning("無法到達路徑點: " + 當前路徑點.name);
            }
        }
    }
}
