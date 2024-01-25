using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Tower : MonoBehaviour, IPointerDownHandler
{
    public ETowerType towerType;
    public Vector2 coordinates;
    public bool freeFire;

    public bool placed;

    [SerializeField] Weapon[] weapons;
    [SerializeField] RectTransform rT;
    [SerializeField] Image whiteImage;
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] TextMeshProUGUI rangeText;

    [SerializeField] Transform[] decors;
    [SerializeField] RectTransform shineRT;


    private Vector2 initialAnchoredPos;

    public static UnityAction<Tower> OnClick;
    private void Start()
    {
        SpawnAnim();
        initialAnchoredPos = rT.anchoredPosition;
    }

    private void SpawnAnim()
    {
        for (int i = 0; i < decors.Length; i++)
        {
            decors[i].DORotate(Vector3.zero, .5f).From(Vector3.forward * (i == 0 ? 180 : -180)).SetEase(Ease.InBack);
        }
        rT.DOBounce(.2f).SetDelay(.5f);
        shineRT.DOAnchorPos(Vector3.up * 200f, 1f).From(Vector2.up * -50).SetDelay(1f).SetRelative().SetEase(Ease.Linear);
    }

    public void Spawn(Vector2 coordinates, bool playSpawnAnim = true)
    {
        this.coordinates = coordinates;
        placed = true;
        Config();
        if (playSpawnAnim)
            SpawnAnim();
    }

    private void OnEnable()
    {
        if (!placed)
            return;

        Config();
    }
    private void OnDisable()
    {
        if (!placed)
            return;

        Enemy.OnMove -= TryShoot;
        foreach (var w in weapons)
        {
            w.OnFire -= OnWeaponFire;
        }
    }
    private void Config()
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
    private void Update()
    {
        SetFreeFire();
    }

    private void OnWeaponFire(Weapon weapon)
    {
        whiteImage.WhiteFlash();

        rT.DOKill();
        rT.anchoredPosition = initialAnchoredPos;

        rT.DOWeaponPunch(weapon.data.side, initialAnchoredPos);
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

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }
}
