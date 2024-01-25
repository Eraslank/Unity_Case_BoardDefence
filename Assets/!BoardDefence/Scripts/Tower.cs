using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Tower : MonoBehaviour
{
    //1     -> Rifle
    //2     -> Revolver
    //3     -> Shotgun
    //4*    -> Sniper

    [SerializeField] Weapon[] weapons;
    [SerializeField] RectTransform rT;
    [SerializeField] Image whiteImage;
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] TextMeshProUGUI rangeText;

    [SerializeField] Transform[] decors;
    [SerializeField] RectTransform shineRT;
    public Vector2 coordinates;

    public bool freeFire;

    private Vector2 initialAnchoredPos;

    private void Start()
    {
        Spawn();
        initialAnchoredPos = rT.anchoredPosition;
        if (coordinates.x != -1)
            GameManager.Instance.RegisterTower(this);
    }

    private void Spawn()
    {
        for (int i = 0; i < decors.Length; i++)
        {
            decors[i].DORotate(Vector3.zero, .5f).From(Vector3.forward * (i == 0 ? 180 : -180)).SetEase(Ease.InBack);
        }
        shineRT.DOAnchorPos(Vector3.up * 200f, 1f).SetDelay(.6f).SetRelative().SetEase(Ease.Linear);
    }

    private void OnEnable()
    {
        Enemy.OnMove += TryShoot;

        foreach (var w in weapons)
        {
            w.OnFire += OnWeaponFire;
            w.coordinates = coordinates;
            damageText.text = w.data.damage.ToString();
            rangeText.text = w.data.range.ToString();
        }
    }
    private void OnDisable()
    {
        Enemy.OnMove -= TryShoot;
        foreach (var w in weapons)
        {
            w.OnFire -= OnWeaponFire;
        }
    }

    private void Update()
    {
        SetFreeFire();
    }

    private void OnWeaponFire(Weapon weapon)
    {
        whiteImage.WhiteFlash();
        rT.DOWeaponPunch(weapon.data.side);
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
        foreach (var w in weapons)
        {
            if (w.freeFire = freeFire)
                w.Fire();
        }
    }

}
