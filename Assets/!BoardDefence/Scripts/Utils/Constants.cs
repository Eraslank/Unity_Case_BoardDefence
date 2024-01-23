using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponData
{
    public ESide side;
    public float damage;
    public float range;
    public float reloadDuration;
}

public enum ESide : int
{
    NONE = -1,

    Up,
    Right,
    Down,
    Left,
}

public static class ESideExtensions
{
    public static Vector2 GetDirection(this ESide side) => side switch
    {
        ESide.Up => Vector2.up,
        ESide.Right => Vector2.right,
        ESide.Down => Vector2.down,
        ESide.Left => Vector2.left,

        _ => default
    };

    public static bool IsTowards(this ESide side, Vector2 rel) => side switch
    {
        ESide.Up => rel.y > 0,
        ESide.Right => rel.x > 0,
        ESide.Down => rel.y < 0,
        ESide.Left => rel.x < 0,

        _ => false
    };
}