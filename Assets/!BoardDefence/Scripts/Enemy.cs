using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public Vector2 coordinates;
    public float health = 10;
    public float currentHealth = 0;
    public float blocksPerSec = 1f;

    [SerializeField] Image whiteImage;
    [SerializeField] RectTransform rT;
    [SerializeField] RectTransform paddingRT;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Image damageBar;
    [SerializeField] CanvasGroup canvasGroup;

    public static UnityAction<Enemy, Vector2> OnMove;

    bool _faded = false;
    private void Start()
    {
        GameManager.Instance.RegisterEnemy(this);
        healthText.text = (currentHealth = health).ToString();
        Move();
        //OnMove += (a, b) => Debug.Log(b);
    }

    public void Move()
    {
        rT.DOAnchorPosY(-rT.rect.height - 50, 1f / blocksPerSec)
          .SetLoops(8, LoopType.Incremental)
          .SetRelative()
          .SetEase(Ease.Linear)
          .OnStepComplete(() =>
          {
              OnMove?.Invoke(this, coordinates += Vector2.up);

              canvasGroup.DOKill();

              if (GameManager.Instance.GetTower(coordinates + Vector2.up))
              {
                  if (_faded)
                      return;
                  canvasGroup.DOFade(.5f, .5f).SetDelay(Mathf.Clamp01(1 - blocksPerSec)).From(1f);
                  _faded = true;
              }
              else if (_faded)
              {
                  canvasGroup.DOFade(1f, .5f).From(.5f);
                  _faded = false;
              }

          });
    }

    public void TakeDamage(float damage)
    {
        whiteImage.DOKill();
        whiteImage.WhiteFlash();
        paddingRT.DOShakeAnchorPos(.25f, 15f, 50).OnComplete(() =>
        {
            paddingRT.DOLocalMove(Vector2.zero, .05f).SetEase(Ease.InQuad);
        });

        currentHealth -= damage;
        healthText.text = currentHealth < 0 ? "0" : $"{currentHealth}";

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameManager.Instance.DeRegisterEnemy(this);

            transform.DOKill();
            StartCoroutine(C_Destroy());
            IEnumerator C_Destroy()
            {
                yield return new WaitForEndOfFrame();
                canvasGroup.DOFade(0.1f, .25f).SetDelay(.75f).OnComplete(() =>
                {
                    Destroy(gameObject);
                });
            }
        }
        damageBar.fillAmount = 1 - (currentHealth / health);
    }
}
