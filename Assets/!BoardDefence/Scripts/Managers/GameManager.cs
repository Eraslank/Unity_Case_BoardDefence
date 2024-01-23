using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public List<Enemy> _enemies = new List<Enemy>();
    public List<Tower> _towers = new List<Tower>();

    private void Awake()
    {
        _enemies.Clear();
        _towers.Clear();
    }
    public void RegisterEnemy(Enemy e)
    {
        _enemies.Add(e);
    }
    public void DeRegisterEnemy(Enemy e)
    {
        if(_enemies.Contains(e))
            _enemies.Remove(e);
    }
    public void RegisterTower(Tower t)
    {
        _towers.Add(t);
    }

    public void Fire(Vector2 from, WeaponData weaponData)
    {
        var checkPos = from + weaponData.side.GetDirection();

        for (int i = 0; i < weaponData.range; i++)
        {
            var possibleEnemy = _enemies.FirstOrDefault(e => e && e.coordinates == checkPos);
            if (possibleEnemy)
            {
                possibleEnemy.TakeDamage(weaponData.damage);
                return;
            }
            checkPos += weaponData.side.GetDirection();
        }
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
}
