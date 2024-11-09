using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private Enemy EnemyGhoulPrefab;
    [SerializeField] private Enemy EnemyAbominationPrefab;

    internal void SpawnEnemy(EnemyType enemyType)
    {
        switch(enemyType)
        {
            case EnemyType.Ghoul:
                Instantiate(EnemyGhoulPrefab, transform);
                break;
            case EnemyType.Abomination:
                Instantiate(EnemyAbominationPrefab, transform);
            break;
        }
    }
}
