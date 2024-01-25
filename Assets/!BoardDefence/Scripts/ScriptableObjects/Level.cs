using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level")]
public class Level : ScriptableObject
{
    [Header("BOARD")]
    public int rows;
    public int columns;

    [Space(20)]
    [Header("DATA")]
    [Tooltip("Delay(sec) between each enemy spawn")]
    public float spawnInterval;
    public bool shuffleEnemyList;
    public EnemyLevelData[] enemySpawnList;
    public TowerLevelData[] towerList;
}

[System.Serializable]
public struct EnemyLevelData
{
    public Enemy enemy;
    public int count;

    [Tooltip("-1 means randomized")]
    public int column;

}

[System.Serializable]
public struct TowerLevelData
{
    public Tower tower;

    public int count;
}