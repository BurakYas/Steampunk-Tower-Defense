using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPortal : MonoBehaviour
{
    [SerializeField] private float spawnCooldown;
    private float spawnTimer;
    [Space]

    [SerializeField] private List<Waypoint> waypointList;

    private List<GameObject> enemiesToCreate = new List<GameObject>();

    private void Awake()
    {
        CollectWaypoints();
    }

    private void Update()
    {
        if (CanMakeNewEnemy())
        {
            CreateEnemy();
        }
    }

    private bool CanMakeNewEnemy()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0 && enemiesToCreate.Count > 0)
        {
            spawnTimer = spawnCooldown;
            return true;
        }

        return false;
    }

    private void CreateEnemy()
    {
        GameObject randomEnemy = GetRandomEnemy();
        GameObject newEnemy = Instantiate(randomEnemy, transform.position, Quaternion.identity); // Instantiate the basic enemy prefab

        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        enemyScript.SetupEnemy(waypointList); // Setup the enemy with the list of waypoints
    }

    private GameObject GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemiesToCreate.Count); // Get a random index from the list of enemies
        GameObject choosenEnemy = enemiesToCreate[randomIndex]; // Get the enemy from the list

        enemiesToCreate.Remove(choosenEnemy);

        return choosenEnemy;
    }

    public void AddEnemy(GameObject enemyToAdd) => enemiesToCreate.Add(enemyToAdd);

    [ContextMenu("Collect Waypoints")]
    private void CollectWaypoints()
    {
        waypointList = new List<Waypoint>();

        foreach (Transform child in transform)
        {
            Waypoint waypoint = child.GetComponent<Waypoint>();

            if (waypoint != null)            
                waypointList.Add(waypoint);            
        }
    }
}
