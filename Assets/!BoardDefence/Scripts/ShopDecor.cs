using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopDecor : MonoBehaviour
{
    [SerializeField] RawImage[] images;
    [SerializeField] TextMeshProUGUI stockText;
    [SerializeField] Image disableImage;

    public ETowerType towerType;

    public bool selected = false;

    public void Select()
    {
        transform.DOKill();

        foreach(var image in images)
            image.DOColor(GameColors.DecorHighlight, .25f);

        transform.DOScale(1.2f, .2f).SetLoops(-1, LoopType.Yoyo);
        selected = true;
    }

    public void DeSelect()
    {
        transform.DOKill();

        foreach(var image in images)
            image.DOColor(Color.white, .25f);

        transform.DOScale(1f, .2f);
        selected = false;
    }

    public void UpdateStock(int stock)
    {
        stockText.text = $"{stock}x";

        if (stock <= 0)
        {
            disableImage.gameObject.SetActive(true);
            disableImage.DOFade(.6f,.5f).From(0);
        }
    }

    public void ResetState()
    {
        disableImage.color = new Color(0,0,0,0);
        disableImage.gameObject.SetActive(false);
    }
}
