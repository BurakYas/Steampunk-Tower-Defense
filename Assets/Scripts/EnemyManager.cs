using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveDetails
{
    public int basicEnemy;
    public int fastEnemy;
}

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private WaveDetails[] levelWaves;
    private int waveIndex;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject basicEnemy;
    [SerializeField] private GameObject fastEnemy;

    private List<EnemyPortal> enemyPortals;

    private void Awake()
    {
        enemyPortals = new List<EnemyPortal>(FindObjectsOfType<EnemyPortal>());
    }

    private void Start()
    {
        SetupNextWave();
    }

    [ContextMenu("Setup Next Wave")]
    private void SetupNextWave()
    {
        List<GameObject> newEnemies = NewEnemyWave();
        int portalIndex = 0;

        if (newEnemies == null)
            return;

        for (int i = 0; i < newEnemies.Count; i++)
        {
            GameObject enemyToAdd = newEnemies[i];
            EnemyPortal portalToRecieveEnemy = enemyPortals[portalIndex];

            portalToRecieveEnemy.AddEnemy(enemyToAdd);

            portalIndex++;

            if (portalIndex >= enemyPortals.Count)            
                portalIndex = 0;            
        }
    }

    private List<GameObject> NewEnemyWave()
    {
        if (waveIndex >= levelWaves.Length)
        {
            Debug.LogWarning("No more waves to spawn");
            return null;
        }

        List<GameObject> newEnemyList = new List<GameObject>(); // Create a new list of enemies

        for (int i = 0; i < levelWaves[waveIndex].basicEnemy; i++)
        {
            newEnemyList.Add(basicEnemy); // Add the basic enemy prefab to the list
        }

        for (int i = 0; i < levelWaves[waveIndex].fastEnemy; i++)
        {
            newEnemyList.Add(fastEnemy); // Add the fast enemy prefab to the list
        }

        waveIndex++;

        return newEnemyList;
    }
}
