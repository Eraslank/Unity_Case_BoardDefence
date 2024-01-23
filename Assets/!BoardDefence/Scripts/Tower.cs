using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tower : MonoBehaviour
{
    //1     -> Rifle
    //2     -> Revolver
    //3     -> Shotgun
    //4*    -> Sniper

    [SerializeField] Weapon[] weapons;
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] TextMeshProUGUI rangeText;
    public Vector2 coordinates;

    public bool freeFire;

    private void Start()
    {
        if(coordinates.x != -1)
            GameManager.Instance.RegisterTower(this);
    }
    private void OnEnable()
    {
        Enemy.OnMove += TryShoot;

        foreach (var w in weapons)
        {
            w.coordinates = coordinates;
            damageText.text = w.data.damage.ToString();
            rangeText.text = w.data.range.ToString();
        }
    }
    private void OnDisable()
    {
        Enemy.OnMove -= TryShoot;
    }

    private void Update()
    {
        SetFreeFire();
    }

    private void TryShoot(Enemy e, Vector2 eCoor)
    {
        if (freeFire)
            return;

        foreach (var weapon in weapons)
        {
            weapon.TryFire(eCoor - coordinates);
        }
    }

    public void SetFreeFire()
    {
        foreach(var w in weapons)
        {
            if(w.freeFire = freeFire)
                w.Fire();
        }
    }

}