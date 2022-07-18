using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public GameObject enemyObj;

    public float spawnRadius;
    public int spawnRate;

    private bool waveCompleted;
    private int enemiesLeftToSpawn;
    private List<int> waves;
    private int wave_ind;
    private float nextSpawnTime;

    public int enemiesRemaining;

    void Start()
    {
        // Initialise wave index
        wave_ind = 0;

        // Define number of enemies in each wave
        waves = new List<int>{1000, 7, 10, 30};

        waveCompleted = true;
    }

    void Update()
    {
        // If there is no current active wave, begin new wave
        if (waveCompleted && wave_ind < waves.Count)
        {   
            Debug.Log("WAVE" + wave_ind);
            waveCompleted = false;     

            int totalEnemiesInWave = waves[wave_ind];

            enemiesRemaining = totalEnemiesInWave;

            StartCoroutine(CreateWave(totalEnemiesInWave));

            wave_ind++;
        }
    }

    IEnumerator CreateWave(int enemiesInWave)
    {
        enemiesLeftToSpawn = enemiesInWave;

        while (enemiesLeftToSpawn > 0)
        {
            // Spawn chunk of enemies
            int spawnNum = (int) Mathf.Round(Random.Range(1,5));
            
            if (spawnNum > enemiesLeftToSpawn)
            {
                spawnNum = enemiesLeftToSpawn;
            }

            SpawnEnemies(spawnNum);

            // Update num of enemies left and wait for next chunk
            enemiesLeftToSpawn -= spawnNum;

            // Don't divide by zero fool
            if (spawnRate == 0)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1/spawnRate);
        }
    }

    void SpawnEnemies(int spawnNum)
    {
        for (int i=0; i<spawnNum; i++)
        {
            // Get random angle and radius to spawn enemy
            float randAngle = Random.Range(0,360);
            float randRadius = Random.Range(spawnRadius-20f, spawnRadius+20f);
            Vector3 pos = transform.position + new Vector3 (randRadius * Mathf.Cos(randAngle), randRadius * Mathf.Sin(randAngle), 0);

            GameObject enemy = Instantiate(enemyObj, pos, Quaternion.Euler(0, 0, randAngle));

            enemy.transform.SetParent(transform);
        }
    }

    public void EnemyDied()
    {
        enemiesRemaining -= 1;
        if (enemiesRemaining ==  0)
        {
            waveCompleted = true;
        }
    }
}
