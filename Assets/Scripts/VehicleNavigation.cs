using System.Linq; // 引入 System.Linq，支持 LINQ 操作符
using UnityEngine;
using UnityEngine.AI;

public class VehicleNavigation : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject[] waypoints;
    public GameObject currentWaypoint;
    private int currentWaypointIndex = 0;
    public float thresholdDistance = 0.5f; // 到達 waypoint 的距離閾值

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false; // 禁用自動更新位置
        agent.updateRotation = false; // 禁用自動旋轉

        // 查找所有帶有標籤 "wayPoints" 的遊戲對象
        waypoints = GameObject.FindGameObjectsWithTag("wayPoints");

        // 設置第一個目標 waypoint
        if (waypoints.Length > 0)
        {
            SortWaypointsByDistance();
            currentWaypoint = waypoints[currentWaypointIndex];
            MoveToNextWaypoint();
        }
    }

    void Update()
    {
        if (currentWaypoint != null)
        {
            float distance = Vector3.Distance(agent.transform.position, currentWaypoint.transform.position);

            if (distance <= thresholdDistance && !agent.pathPending)
            {
                Destroy(currentWaypoint);
                GetNextWaypoint();
            }
        }
    }

    public void MoveToNextWaypoint()
    {
        if (currentWaypoint != null)
        {
            agent.SetDestination(currentWaypoint.transform.position);
        }
    }

    public void GetNextWaypoint()
    {
        currentWaypointIndex++;
        if (currentWaypointIndex < waypoints.Length)
        {
            currentWaypoint = waypoints[currentWaypointIndex];
            MoveToNextWaypoint();
        }
    }

    void SortWaypointsByDistance()
    {
        // 使用 LINQ 按照距離排序所有的 waypoints
        waypoints = waypoints
            .OrderBy(waypoint => Vector3.Distance(agent.transform.position, waypoint.transform.position))
            .ToArray();
    }

    public GameObject FindClosestForwardWaypoint(Vector3 currentPosition, Vector3 currentForward)
    {
        GameObject closestWaypoint = null;
        float minDistance = float.MaxValue;

        foreach (var waypoint in waypoints)
        {
            if (waypoint == null) continue;

            Vector3 toWaypoint = (waypoint.transform.position - currentPosition).normalized;
            if (Vector3.Dot(currentForward, toWaypoint) > 0)
            {
                float distance = Vector3.Distance(currentPosition, waypoint.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestWaypoint = waypoint;
                }
            }
        }
        return closestWaypoint;
    }
}
