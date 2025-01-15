using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { Basic, Fast, None}

public class Enemy : MonoBehaviour, IDamagable
{
    private NavMeshAgent agent;

    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Transform centerPoint;
    public int healthPoints = 4;

    [Header("Movement")]
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private Transform[] waypoints;
    private int waypointIndex;

    private float totalDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.avoidancePriority = Mathf.RoundToInt(agent.speed * 10); // Set the avoidance priority based on the speed
    }

    private void Start()
    {
        waypoints = FindFirstObjectByType<WaypointManager>().GetWaypoints(); // Get the waypoints from the WaypointManager

        CollectTotalDistance();
    }

    private void Update()
    {
        FaceTarget(agent.steeringTarget);

        // Check if the agent has reached the destination
        if (agent.remainingDistance < .5f)
        {
            // Check if the agent has reached the last waypoint
            agent.SetDestination(GetNexWaypoint());
        }
    }

    public float DistanceToFinishLine() => totalDistance + agent.remainingDistance; // Return the distance to the finish line

    private void CollectTotalDistance()
    {
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            float distance = Vector3.Distance(waypoints[i].position, waypoints[i + 1].position); // Calculate the total distance between waypoints
            totalDistance += distance;
        }
    }

    private void FaceTarget(Vector3 newTarget)
    {
        Vector3 directionTarget = newTarget - transform.position; // Get the direction to the target
        directionTarget.y = 0; // Set the y axis to 0

        Quaternion newRotation = Quaternion.LookRotation(directionTarget); // Get the rotation to look at the target

        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, turnSpeed * Time.deltaTime); // Rotate the enemy to look at the target smoothly
    }

    private Vector3 GetNexWaypoint()
    {
        if (waypointIndex >= waypoints.Length)
        {
            //waypointIndex = 0; // Reset the waypoint index
            return transform.position;
        }

        Vector3 targetPoint = waypoints[waypointIndex].position; // Get the next waypoint position

        if (waypointIndex > 0)
        {
            float distance = Vector3.Distance(waypoints[waypointIndex - 1].position, waypoints[waypointIndex].position);
            totalDistance -= distance; // Subtract the distance from the total distance
        }

        waypointIndex++; // Increase the waypoint index

        return targetPoint;
    }

    public Vector3 CenterPoint() => centerPoint.position;

    public EnemyType GetEnemyType() => enemyType;

    public void TakeDamage(int damage)
    {
        healthPoints = healthPoints - damage;

        if (healthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}
