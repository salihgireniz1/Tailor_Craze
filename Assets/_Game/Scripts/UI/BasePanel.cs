using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BasePanel : MonoBehaviour
{
    CanvasGroup _cg;
    CanvasGroup cg => _cg ?? (_cg = GetComponent<CanvasGroup>());

    [BoxGroup("Base Panel"), SerializeField] float appearTime = 0.5f, appearDelay = 0f;
    [BoxGroup("Base Panel"), SerializeField] float disappearTime = 0.5f, disappearDelay = 0f;
    [BoxGroup("Base Panel"), ReorderableList] public List<GameState> appearStates;

    protected void Awake()
    {
        gameObject.SetActive(false);
        cg.alpha = 0f;
        cg.interactable = cg.blocksRaycasts = true;
    }

    public virtual float Appear(float delay = 0f)
    {
        gameObject?.SetActive(true);
        cg.DOFade(1f, appearTime)
            .SetDelay(delay + appearDelay)
            .SetUpdate(true)
            .SetEase(Ease.Linear)
            .OnStart(OnAppearStart)
            .OnComplete(() =>
            {
                cg.interactable = cg.blocksRaycasts = true;
                OnAppearEnd();
            })
            .Play();
        return appearTime + appearDelay;
    }

    public virtual float Disappear(float delay = 0f)
    {
        cg.DOFade(0f, disappearTime)
            .SetDelay(delay + disappearDelay)
            .SetUpdate(true)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                cg.interactable = cg.blocksRaycasts = false;
                OnDisappearStart();
            })
            .OnComplete(() =>
            {
                OnDisappearEnd();
                gameObject.SetActive(false);
            })
            .Play();
        return disappearTime + disappearDelay;
    }

    public virtual void OnAppearStart() { }
    public virtual void OnAppearEnd() { }
    public virtual void OnDisappearStart() { }
    public virtual void OnDisappearEnd() { }
}