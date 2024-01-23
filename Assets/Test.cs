using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] Transform target;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log(transform.position + " " + target.position);
        var rt = transform.GetComponent<RectTransform>();
        rt.DOAnchorPosY(-rt.rect.height, 1f / .5f).SetLoops(7,LoopType.Incremental).SetRelative().SetEase(Ease.Linear);
        InvokeRepeating(nameof(TestA), 0f, 1f);
    }
    int x = 0;
    void TestA()
    {
        Debug.Log(x++);
    }
}
