using System.Linq; // �ޤJ System.Linq�A��� LINQ �ާ@��
using UnityEngine;
using UnityEngine.AI;

public class VehicleNavigation : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject[] waypoints;
    public GameObject currentWaypoint;
    private int currentWaypointIndex = 0;
    public float thresholdDistance = 0.5f; // ��F waypoint ���Z���H��

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false; // �T�Φ۰ʧ�s��m
        agent.updateRotation = false; // �T�Φ۰ʱ���

        // �d��Ҧ��a������ "wayPoints" ���C����H
        waypoints = GameObject.FindGameObjectsWithTag("wayPoints");

        // �]�m�Ĥ@�ӥؼ� waypoint
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
        // �ϥ� LINQ ���ӶZ���ƧǩҦ��� waypoints
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
