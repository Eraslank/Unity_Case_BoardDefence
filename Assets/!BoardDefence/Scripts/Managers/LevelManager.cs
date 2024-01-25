using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviourSingleton<LevelManager>
{
    [SerializeField] Level[] levels;

    [SerializeField] Node nodePrefab;
    [SerializeField] FlexibleGridLayout nodeHolder;

    private Level activeLevel;

    public Level GetLevel() => activeLevel;

    public void GenerateLevel(int levelId)
    {
        activeLevel = levels[levelId];

        nodeHolder.transform.DestroyAllChildren();

        nodeHolder.rows = activeLevel.rows;
        nodeHolder.columns = activeLevel.columns;

        for (int i = 0; i < activeLevel.rows; i++)
        {
            for(int j = 0; j < activeLevel.columns; j++)
            {
                var n = Instantiate(nodePrefab, nodeHolder.transform);
                n.coordinates = new Vector2(j, i);
            }
        }
    }

    public void AutoGenerateLevel()
    {
        int levelId = PlayerPrefs.GetInt("LevelId", 0);
        GenerateLevel(levelId);
    }
}
