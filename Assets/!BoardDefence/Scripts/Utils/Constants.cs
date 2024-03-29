using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public struct GameColors
{
    public static Color Enemy { get { return new Color(.22f, .22f, .22f); } }
    public static Color Tower { get { return new Color(0, .6f, 1f); } }
    public static Color DecorHighlight { get { return new Color(.1f, 1f, 0); } }
}


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
public enum EEnemyType : int
{
    NONE = -1,

    One,
    Two,
    Three,
}
public enum ETowerType : int
{
    NONE = -1,

    One,
    Two,
    Three,
}

public static class ESideExtensions
{
    public static int Int(this ESide i) => (int)i;
    public static Vector2 GetDirection(this ESide side) => side switch
    {
        ESide.Up => Vector2.down,
        ESide.Right => Vector2.right,
        ESide.Down => Vector2.up,
        ESide.Left => Vector2.left,

        _ => default
    };
    public static bool IsSigned(this ESide side) => side switch
    {
        ESide.Up => true,
        ESide.Right => false,
        ESide.Down => true,
        ESide.Left => true,

        _ => default
    };

    public static bool IsTowards(this ESide side, Vector2 rel) => side switch
    {
        ESide.Up => rel.y < 0,
        ESide.Right => rel.x > 0,
        ESide.Down => rel.y > 0,
        ESide.Left => rel.x < 0,

        _ => false
    };
}

public static class ImageExtensions
{
    public static Tween WhiteFlash(this Image i, float duration = .25f)
    {
        return i.DOColor(new Color(), duration).From(Color.white);
    }
}

public static class RectTransformExtensions
{
    public static void DOWeaponPunch(this RectTransform rT, ESide side, Vector3? initialPos = null)
    {

        bool sign = Random.value > 0.5f;

        var mov = new Vector2(sign ? -15 : 15, -40);

        for (int i = 0; i < side.Int(); i++)
        {
            mov = Quaternion.Euler(0, 0, -90) * mov;
        }

        var rot = Vector3.forward * (sign ? -7.5f : 7.5f);

        rT.DOAnchorPos(mov, .02f).SetRelative().SetEase(Ease.OutCirc);
        rT.DORotate(rot, .1f).SetEase(Ease.OutCirc);
        rT.DOScale(.9f, .1f);

        DOVirtual.DelayedCall(.25f, () =>
        {
            if (initialPos.HasValue)
                rT.DOAnchorPos(initialPos.Value, .1f).SetEase(Ease.OutCirc);
            else
                rT.DOAnchorPos(-mov, .1f).SetRelative().SetEase(Ease.OutCirc);

            rT.DORotate(Vector3.zero, .1f).SetEase(Ease.OutCirc);
            rT.DOBounce();
        });
    }
    public static Tween DOBounce(this RectTransform rT, float duration = .1f)
    {
        return rT.DOScale(1.15f, duration).OnComplete(() =>
        {
            rT.DOScale(.9f, duration * .5f).OnComplete(() =>
            {
                rT.DOScale(1, duration);
            });
        });
    }
}