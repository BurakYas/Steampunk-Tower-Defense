using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPortal : MonoBehaviour
{    
    [SerializeField] private float spawnCooldown;
    private float spawnTimer;

    public List<GameObject> enemiesToCreate;

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
    }

    private GameObject GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemiesToCreate.Count); // Get a random index from the list of enemies
        GameObject choosenEnemy = enemiesToCreate[randomIndex]; // Get the enemy from the list

        enemiesToCreate.Remove(choosenEnemy);

        return choosenEnemy;
    }

    public List<GameObject> GetEnemyList() => enemiesToCreate;
}
