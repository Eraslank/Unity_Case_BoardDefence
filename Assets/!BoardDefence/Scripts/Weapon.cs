using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public WeaponData data;
    public bool freeFire;

    [SerializeField] Image cooldownImage;

    internal Vector2 coordinates;

    public bool _readyToShoot = true;
    private Vector2 _shootDir = Vector2.zero;

    public UnityAction<Weapon> OnFire;
    public UnityAction<Weapon> OnReload;

    private void Awake()
    {
        _shootDir = data.side.GetDirection() * data.range;
    }

    public bool IsInRange(Vector2 relPos)
    {
        if(!data.side.IsTowards(relPos)) //Already Past Weapon
            return false;

        if (relPos.x == 0 && relPos.y == 0) //Both on Same Node
            return false;

        if (relPos.x != 0 && relPos.y != 0) //Diagonal
            return false;

        if (_shootDir.x == relPos.x)
            return Mathf.Abs(_shootDir.y) >= Mathf.Abs(relPos.y);
        else if(_shootDir.y == relPos.y)
            return Mathf.Abs(_shootDir.x) >= Mathf.Abs(relPos.x);

        return false;
    }

    public void Fire()
    {
        if (!_readyToShoot)
            return;

        if (!GameManager.Instance.Fire(coordinates, data))
            return;

        _readyToShoot = false;


        Invoke(nameof(Reload), data.reloadDuration);

        cooldownImage.DOFillAmount(0,data.reloadDuration)
                     .From(1)
                     .SetEase(Ease.Linear);

        OnFire?.Invoke(this);
    }

    public void TryFire(Vector2 relPos)
    {
        if (_readyToShoot && IsInRange(relPos))
            Fire();
    }

    public void Reload()
    {
        _readyToShoot = true;
        OnReload?.Invoke(this);

        if (freeFire)
            Fire();
    }
}
