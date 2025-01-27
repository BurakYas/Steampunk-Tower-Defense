using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { Basic, Fast, None}

public class Enemy : MonoBehaviour, IDamagable
{
    private EnemyPortal myPortal;
    private NavMeshAgent agent;

    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Transform centerPoint;
    public int healthPoints = 4;

    [Header("Movement")]
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private List<Transform> myWayPoints;
    private int nextWaypointIndex;
    private int currentWaypointIndex;

    private float totalDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.avoidancePriority = Mathf.RoundToInt(agent.speed * 10); // Set the avoidance priority based on the speed
    }
    
    public void SetupEnemy(List<Waypoint> newWaypoints,EnemyPortal myNewPortal)
    {
        myWayPoints = new List<Transform>();

        foreach (var point in newWaypoints)
        {
            myWayPoints.Add(point.transform);
        }

        CollectTotalDistance();

        myPortal = myNewPortal;
    }

    private void Update()
    {
        FaceTarget(agent.steeringTarget);

        // Check if the agent has reached the destination
        if (ShouldChangeWaypoint())
        {
            // Check if the agent has reached the last waypoint
            agent.SetDestination(GetNexWaypoint());
        }
    }

    private bool ShouldChangeWaypoint()
    {
        if (nextWaypointIndex >= myWayPoints.Count)
            return false;

        if (agent.remainingDistance < .5f)
            return true;

        Vector3 currentWaypoint = myWayPoints[currentWaypointIndex].position;
        Vector3 nextWaypoint = myWayPoints[nextWaypointIndex].position;

        float distanceToNextWaypoint = Vector3.Distance(transform.position, nextWaypoint);
        float distanceBetweenWaypoints = Vector3.Distance(currentWaypoint, nextWaypoint);

        return distanceBetweenWaypoints > distanceToNextWaypoint;
    }

    public float DistanceToFinishLine() => totalDistance + agent.remainingDistance; // Return the distance to the finish line

    private void CollectTotalDistance()
    {
        for (int i = 0; i < myWayPoints.Count - 1; i++)
        {
            float distance = Vector3.Distance(myWayPoints[i].position, myWayPoints[i + 1].position); // Calculate the total distance between waypoints
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
        if (nextWaypointIndex >= myWayPoints.Count)
        {
            //waypointIndex = 0; // Reset the waypoint index
            return transform.position;
        }

        Vector3 targetPoint = myWayPoints[nextWaypointIndex].position; // Get the next waypoint position

        if (nextWaypointIndex > 0)
        {
            float distance = Vector3.Distance(myWayPoints[nextWaypointIndex - 1].position, myWayPoints[nextWaypointIndex].position);
            totalDistance -= distance; // Subtract the distance from the total distance
        }

        nextWaypointIndex = nextWaypointIndex + 1; // Increase the waypoint index
        currentWaypointIndex = nextWaypointIndex - 1;

        return targetPoint;
    }

    public Vector3 CenterPoint() => centerPoint.position;

    public EnemyType GetEnemyType() => enemyType;

    public void TakeDamage(int damage)
    {
        healthPoints = healthPoints - damage;

        if (healthPoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        myPortal.RemoveActiveEnemy(gameObject);
        Destroy(gameObject);
    }
}
