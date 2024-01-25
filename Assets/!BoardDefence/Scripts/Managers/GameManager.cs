using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    private List<Enemy> _enemies = new List<Enemy>();
    private List<Tower> _towers = new List<Tower>();

    private List<EnemyLevelData> enemySpawnList;
    private List<TowerLevelData> towerStockList;

    private bool spawningEnemies = false;

    private Tower selectedTower;
    private Node[] placeableNodes;

    [SerializeField] Enemy[] enemyPrefabs;
    [SerializeField] ShopDecor[] decors;


    [SerializeField] Transform towerHolder;
    [SerializeField] Transform enemyHolder;

    [SerializeField] CanvasGroup levelEndPage;

    [SerializeField] TextMeshProUGUI levelEndText;


    bool win = false;
    bool gameComplete = false;
    private void Awake()
    {
        _enemies.Clear();
        _towers.Clear();
    }

    private void Start()
    {
        LevelManager.Instance.AutoGenerateLevel();
    }
    private void ResetGameState()
    {
        win = false;
        gameComplete = false;
        spawningEnemies = false;
        levelEndPage.gameObject.SetActive(false);
        enemyHolder.DestroyAllChildren();
        towerHolder.DestroyAllChildren();

        _enemies.Clear();
        _towers.Clear();

        foreach (var decor in decors)
            decor.ResetState();
    }
    public void OnRestartClick()
    {
        ResetGameState();
        LevelManager.Instance.AutoGenerateLevel();
    }
    public void OnNextClick()
    {
        ResetGameState();
        LevelManager.Instance.NextLevel();
    }
    private void OnEnable()
    {
        Enemy.OnDie += OnEnemyDie;
        Enemy.OnMoveComplete += OnEnemyMoveComplete;

        Tower.OnClick += OnTowerClick;
        Node.OnClick += OnNodeClick;
        LevelManager.OnLevelGenerated += OnLevelGenerated;
    }


    private void OnDisable()
    {
        Enemy.OnDie -= OnEnemyDie;
        Enemy.OnMoveComplete -= OnEnemyMoveComplete;

        Tower.OnClick -= OnTowerClick;
        Node.OnClick -= OnNodeClick;
        LevelManager.OnLevelGenerated -= OnLevelGenerated;
    }

    private void OnEnemyDie(Enemy enemy)
    {
        DeRegisterEnemy(enemy);

        if (gameComplete)
            return;

        if (_enemies.Count <= 0 && enemySpawnList.Count <= 0)
        {
            win = true;
            gameComplete = true;
            Invoke(nameof(ShowLevelEndPage), 1.5f);
        }
    }
    private void OnEnemyMoveComplete(Enemy enemy)
    {
        if (gameComplete)
            return;
        gameComplete = true;
        Invoke(nameof(ShowLevelEndPage), 1.5f);
    }
    private void OnTowerClick(Tower tower)
    {
        selectedTower = null;

        if (tower.placed)
        {
            //Freefire
            return;
        }

        if (!towerStockList.Any(t => t.towerType == tower.towerType))
            return;

        selectedTower = tower;

        var previouslySelected = decors.FirstOrDefault(x => x.selected);
        decors.FirstOrDefault(d => d.towerType == tower.towerType).Select();

        if (previouslySelected)
        {
            previouslySelected.DeSelect();

            if (previouslySelected.towerType == tower.towerType)
            {
                selectedTower = null;
                HighLightPlaceableNodes(false);
            }
        }


        bool? highLight = null;
        if (selectedTower && !Node.HIGHLIGHT_PLACEABLES)
            highLight = true;
        else if (!selectedTower && Node.HIGHLIGHT_PLACEABLES)
            highLight = false;

        if (highLight.HasValue)
        {
            HighLightPlaceableNodes(highLight.Value);
        }
    }

    private void OnNodeClick(Node node)
    {
        if (!selectedTower)
            return;

        var t = Instantiate(selectedTower, node.transform);

        t.GetRectTransform().anchoredPosition3D = Vector3.zero;
        t.Spawn(node.coordinates);

        t.transform.SetParent(towerHolder);

        RegisterTower(t);

        var stock = towerStockList.FirstOrDefault(x => x.towerType == t.towerType);

        --stock.count;
        decors.First(d => d.selected).UpdateStock(stock.count);

        if (stock.count <= 0)
        {
            towerStockList.Remove(stock);
            selectedTower = null;
            decors.First(d => d.selected).DeSelect();

            HighLightPlaceableNodes(false);
        }

        if (!spawningEnemies)
        {
            spawningEnemies = true;
            BeginLevel();
        }

        node.clickable = false;
    }
    private void OnLevelGenerated()
    {
        var l = LevelManager.Instance.GetLevel();

        towerStockList = l.towerList.Select(t => new TowerLevelData() { count = t.count, towerType = t.towerType }).ToList();
        enemySpawnList = l.enemySpawnList.Select(e => new EnemyLevelData() { count = e.count, enemyType = e.enemyType, column = e.column }).ToList();

        placeableNodes = LevelManager.Instance.GetNodes().Where(n => n.coordinates.y >= l.rows / 2).ToArray();

        foreach (var decor in decors)
        {
            decor.UpdateStock(towerStockList.First(t => t.towerType == decor.towerType).count);
        }
    }
    private void BeginLevel()
    {
        StartCoroutine(C_EnemySpawner());
    }

    private IEnumerator C_EnemySpawner()
    {
        var level = LevelManager.Instance.GetLevel();
        var nodes = LevelManager.Instance.GetNodes();
        var interval = new WaitForSeconds(level.spawnInterval);

        do
        {
            EnemyLevelData enemyData;

            if (level.shuffleEnemyList)
                enemyData = enemySpawnList.RandomItem();
            else
                enemyData = enemySpawnList.First();

            var desiredColumn = enemyData.column;
            if (desiredColumn < 0 || desiredColumn > level.columns)
                desiredColumn = UnityEngine.Random.Range(0, level.columns);

            var parent = nodes.FirstOrDefault(n => n.coordinates == new Vector2(desiredColumn, 0));


            var enemy = Instantiate(enemyPrefabs.First(e => e.enemyType == enemyData.enemyType), parent.transform);
            enemy.GetRectTransform().anchoredPosition3D = Vector3.zero;
            enemy.transform.SetParent(enemyHolder);
            enemy.Spawn(parent.coordinates);
            RegisterEnemy(enemy);

            if (--enemyData.count <= 0)
                enemySpawnList.Remove(enemyData);

            yield return interval;

        } while (enemySpawnList.Count > 0);
    }

    public void RegisterEnemy(Enemy e)
    {
        _enemies.Add(e);
    }
    public void DeRegisterEnemy(Enemy e)
    {
        if (_enemies.Contains(e))
            _enemies.Remove(e);
    }
    public void RegisterTower(Tower t)
    {
        _towers.Add(t);
    }

    public bool Fire(Vector2 from, WeaponData weaponData)
    {
        var level = LevelManager.Instance.GetLevel();
        var checkPos = from + weaponData.side.GetDirection();

        for (int i = 0; i < weaponData.range; i++)
        {

            if (checkPos.x < 0 || checkPos.x >= level.columns) //Outside Columns
                return false;
            if (checkPos.y < 0 || checkPos.y >= level.rows) //Outside Rows
                return false;

            var possibleEnemy = _enemies.FirstOrDefault(e => e && e.coordinates == checkPos);
            if (possibleEnemy)
            {
                possibleEnemy.TakeDamage(weaponData.damage);
                return true;
            }
            checkPos += weaponData.side.GetDirection();
        }

        return false;
    }

    public bool TryGetTower(Vector2 coor, out Tower t)
    {
        t = _towers.FirstOrDefault(x => x.coordinates == coor);
        return t;
    }

    public Tower GetTower(Vector2 coor)
    {
        return _towers.FirstOrDefault(x => x.coordinates == coor);
    }

    private void HighLightPlaceableNodes(bool state)
    {
        foreach (var n in placeableNodes)
        {
            n.HighLight(state);
        }
    }
    private void ShowLevelEndPage()
    {
        levelEndPage.gameObject.SetActive(true);
        levelEndPage.DOFade(1, .5f).From(0);

        levelEndText.text = $"You {(win ? "Win" : "Lose")}!";
    }
}
