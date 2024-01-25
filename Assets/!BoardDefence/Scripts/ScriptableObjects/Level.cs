using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level")]
public class Level : ScriptableObject
{
    [Header("BOARD")]
    [Range(4, 8)] public int rows;
    [Range(1, 5)] public int columns;

    [Space(20)]
    [Header("DATA")]
    [Tooltip("Delay(sec) between each enemy spawn")]
    public float spawnInterval;
    public bool shuffleEnemyList;
    public EnemyLevelData[] enemySpawnList;
    public TowerLevelData[] towerList;
}

[System.Serializable]
public class EnemyLevelData
{
    public EEnemyType enemyType;
    public int count;

    [Tooltip("-1 means randomized")]
    public int column;

}

[System.Serializable]
public class TowerLevelData
{
    public ETowerType towerType;

    public int count;
}