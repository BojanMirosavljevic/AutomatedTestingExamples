using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : StaticInstance<GameManager>
{
    public PlayerController Player;
    public Transform Environment;

    [SerializeField] private List<SpawnPoint> SpawnPoints;
    [SerializeField] private GameObject GameEndedUI;
    [SerializeField] private TextMeshProUGUI LevelProgressText;

    private float timePassedSinceLastSpawn = 0f;
    private float randomizedTimeToSpawn = float.MaxValue;

    private int waveCount = 0;
    private int enemiesCount = 0;

    private bool gameEnded = false;

    public event Action<bool> GameEnded;

    private void Start()
    {
        SetRandomizedTimeToSpawn();
        SetLevelProgressText();
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

            SetLevelProgressText();
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

    private void SetLevelProgressText()
    {
        LevelProgressText.text = "Level " + GameSystem.Instance.CurrentLevelNumber + " / Wave: " + waveCount;
    }

    private void SpawnWave()
    {
        waveCount++;

        int randomizedSpawnAmount = Random.Range(GameSystem.Instance.LoadedLevelConfig.SpawnAmountMin, GameSystem.Instance.LoadedLevelConfig.SpawnAmountMax+1);

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
            StartEndGameFlow();

            //GameEnded is invoked once and then clears all actions that were subscribed
            GameEnded?.Invoke(true);
            GameEnded = null;
        }
    }

    public void NotifyPlayerCollided()
    {
        StartEndGameFlow();

        //GameEnded is invoked once and then clears all actions that were subscribed
        GameEnded?.Invoke(false);
        GameEnded = null;
    }

    private void StartEndGameFlow()
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
