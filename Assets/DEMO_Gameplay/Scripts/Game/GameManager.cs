using System.Collections.Generic;
using UnityEngine;

public class GameManager : StaticInstance<GameManager>
{
    public PlayerController Player;
    public Transform Environment;
    public List<SpawnPoint> SpawnPoints;
    public GameObject GameEndedUI;

    private float timePassedSinceLastSpawn = 0f;

    private float randomizedTimeToSpawn = float.MaxValue;

    private int waveCount = 0;
    private int enemiesCount = 0;

    private bool gameEnded = false;
    public bool GameWon = false;

    private void Start()
    {
        SetRandomizedTimeToSpawn();
    }

    private void Update()
    {
        if (gameEnded)
        {
            return;
        }

        timePassedSinceLastSpawn += Time.deltaTime;

        if (timePassedSinceLastSpawn > randomizedTimeToSpawn)
        {
            SpawnWave();

            timePassedSinceLastSpawn = 0f;
            SetRandomizedTimeToSpawn();
        }
    }

    private void SetRandomizedTimeToSpawn()
    {
        if (waveCount == GameSystem.Instance.LoadedLevelConfig.WavesCount)
        {
            randomizedTimeToSpawn = float.MaxValue;
        }
        else
        {
            randomizedTimeToSpawn = Random.Range(GameSystem.Instance.LoadedLevelConfig.SpawnTimeMin, GameSystem.Instance.LoadedLevelConfig.SpawnTimeMax);
        }
    }

    private void SpawnWave()
    {
        waveCount++;

        int randomizedSpawnAmount = Random.Range(GameSystem.Instance.LoadedLevelConfig.SpawnAmountMin, GameSystem.Instance.LoadedLevelConfig.SpawnAmountMax);

        List<int> spawnPointIndexes = new List<int>() { 0, 1, 2, 3, 4, 5 };

        //check spawn abomination
        if (GameSystem.Instance.LoadedLevelConfig.AbominationFrequency > 0 &&
            waveCount % GameSystem.Instance.LoadedLevelConfig.AbominationFrequency == 0)
        {
            int spawnIndex = Random.Range(0, spawnPointIndexes.Count);
            SpawnPoints[spawnPointIndexes[spawnIndex]].SpawnEnemy(EnemyType.Abomination);
            spawnPointIndexes.RemoveAt(spawnIndex);

            enemiesCount++;
            randomizedSpawnAmount--;
        }

        //spawn ghouls
        for (int i = 0; i < randomizedSpawnAmount; i++)
        {
            int spawnIndex = Random.Range(0, spawnPointIndexes.Count);
            SpawnPoints[spawnPointIndexes[spawnIndex]].SpawnEnemy(EnemyType.Ghoul);
            spawnPointIndexes.RemoveAt(spawnIndex);

            enemiesCount++;
        }
    }

    public void NotifyEnemyDestroyed()
    {
        enemiesCount--;

        if (enemiesCount == 0 && waveCount == GameSystem.Instance.LoadedLevelConfig.WavesCount)
        {
            GameWon = true;
            StartEndGameFlow();
        }
    }

    public void StartEndGameFlow()
    {
        if (!gameEnded)
        {
            gameEnded = true;
            Time.timeScale = 0f;
            GameEndedUI.SetActive(true);
        }
    }

    public void GoToMenu()
    {
        SceneSystem.Instance.LoadScene(Scene.Menu, () => {
            Time.timeScale = 1f;
        });
    }
}
