using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private Transform[] waypoints;
    private int waypointIndex;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.avoidancePriority = Mathf.RoundToInt(agent.speed * 10); // Set the avoidance priority based on the speed
    }

    private void Start()
    {
        waypoints = FindFirstObjectByType<WaypointManager>().GetWaypoints(); // Get the waypoints from the WaypointManager
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
        waypointIndex++; // Increase the waypoint index

        return targetPoint;
    }
}
