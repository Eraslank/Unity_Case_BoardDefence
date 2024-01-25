using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviourSingleton<LevelManager>
{
    [SerializeField] Level[] levels;

    [SerializeField] Node nodePrefab;
    [SerializeField] FlexibleGridLayout nodeHolder;

    [SerializeField] TextMeshProUGUI levelText;

    public static UnityAction OnLevelGenerated;

    private Level activeLevel;
    public Level GetLevel() => activeLevel;

    private List<Node> activeNodes = new List<Node>();
    public List<Node> GetNodes() => activeNodes;

    public void GenerateLevel(int levelId)
    {
        activeNodes.Clear();
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
                activeNodes.Add(n);
            }
        }

        levelText.text = $"Level {levelId + 1}";

        OnLevelGenerated?.Invoke();
    }

    public void AutoGenerateLevel()
    {
        int levelId = PlayerPrefs.GetInt("LevelId", 0);
        GenerateLevel(levelId);
    }
    public void NextLevel()
    {
        int levelId = PlayerPrefs.GetInt("LevelId", -1);

        if (++levelId >= levels.Length)
            levelId = 0;

        PlayerPrefs.SetInt("LevelId", levelId);

        GenerateLevel(levelId);
    }
}
