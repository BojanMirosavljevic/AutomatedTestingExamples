using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : StaticInstance<GameManager>
{
    public PlayerController Player;
    public Transform Environment;

    [SerializeField] private List<SpawnPoint> SpawnPoints;
    [SerializeField] private GameObject GameEndedUI;
    [SerializeField] private TextMeshProUGUI LevelProgressText;

    private float timePassedSinceLastSpawn = 0f;
    private float randomizedTimeToSpawn = float.MaxValue;

    public int WaveCount = 0;
    public int EnemiesCount = 0;

    private bool gameEnded = false;

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

    public void SetRandomizedTimeToSpawn()
    {
        if (WaveCount == GameSystem.Instance.LoadedLevelConfig.WavesCount)
        {
            randomizedTimeToSpawn = float.MaxValue;
        }
        else
        {
            randomizedTimeToSpawn = Random.Range(GameSystem.Instance.LoadedLevelConfig.SpawnTimeMin, GameSystem.Instance.LoadedLevelConfig.SpawnTimeMax);
        }
    }

    public void SetLevelProgressText()
    {
        LevelProgressText.text = "Level " + GameSystem.Instance.CurrentLevelNumber + " / Wave: " + WaveCount;
    }

    private void SpawnWave()
    {
        if (WaveCount >= GameSystem.Instance.LoadedLevelConfig.WavesCount)
        {
            return;
        }
        WaveCount++;

        int randomizedSpawnAmount = Random.Range(GameSystem.Instance.LoadedLevelConfig.SpawnAmountMin, GameSystem.Instance.LoadedLevelConfig.SpawnAmountMax+1);

        List<int> spawnPointIndexes = new List<int>() { 0, 1, 2, 3, 4, 5 };

        //check spawn abomination
        if (GameSystem.Instance.LoadedLevelConfig.AbominationFrequency > 0 &&
            WaveCount % GameSystem.Instance.LoadedLevelConfig.AbominationFrequency == 0)
        {
            int spawnIndex = Random.Range(0, spawnPointIndexes.Count);
            SpawnPoints[spawnPointIndexes[spawnIndex]].SpawnEnemy(EnemyType.Abomination);
            spawnPointIndexes.RemoveAt(spawnIndex);

            EnemiesCount++;
            randomizedSpawnAmount--;
        }

        //spawn ghouls
        for (int i = 0; i < randomizedSpawnAmount; i++)
        {
            int spawnIndex = Random.Range(0, spawnPointIndexes.Count);
            SpawnPoints[spawnPointIndexes[spawnIndex]].SpawnEnemy(EnemyType.Ghoul);
            spawnPointIndexes.RemoveAt(spawnIndex);

            EnemiesCount++;
        }
    }

    public void NotifyEnemyDestroyed()
    {
        EnemiesCount--;

        if (EnemiesCount == 0 && WaveCount == GameSystem.Instance.LoadedLevelConfig.WavesCount)
        {
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
