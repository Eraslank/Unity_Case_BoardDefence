using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Node : MonoBehaviour, IPointerDownHandler
{
    public Vector2 coordinates;

    public bool clickable = true;

    [SerializeField] Image highlightImage;

    [SerializeField] bool _autoSetCoordinates;

    public static UnityAction<Node> OnClick;

    public static bool HIGHLIGHT_PLACEABLES = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!clickable)
            return;

        OnClick?.Invoke(this);
    }

    private void OnValidate()
    {
        if (_autoSetCoordinates)
            _autoSetCoordinates = false;

        int siblingIndex = transform.GetSiblingIndex();
        coordinates = new Vector2(siblingIndex % 4, siblingIndex / 4);
    }

    public void HighLight(bool state)
    {
        highlightImage.DOKill();

        if (state)
            highlightImage.DOColor(GameColors.DecorHighlight, 1f).From(new Color()).SetLoops(-1, LoopType.Yoyo);
        else
            highlightImage.DOColor(new Color(), .5f);
    }
}