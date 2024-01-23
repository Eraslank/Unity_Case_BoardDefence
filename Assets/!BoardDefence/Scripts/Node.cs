using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector2 coordinates;

    [SerializeField] bool _autoSetCoordinates;
    private void OnValidate()
    {
        if (_autoSetCoordinates)
            _autoSetCoordinates = false;

        int siblingIndex = transform.GetSiblingIndex();
        coordinates = new Vector2(siblingIndex % 4, siblingIndex / 4);
    }
}