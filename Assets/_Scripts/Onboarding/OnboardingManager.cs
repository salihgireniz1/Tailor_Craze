using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class OnboardingManager : MonoSingleton<OnboardingManager>
{
    public GameObject pointer;
    public GameObject panel;
    public bool IsOnboarded
    {
        get => ES3.Load("onboarded", false);
        set => ES3.Save("onboarded", value);
    }
    [Button]
    void Reset()
    {
        IsOnboarded = false;
    }

    private void Start()
    {
        if (!IsOnboarded)
        {
            pointer.SetActive(true);
        }
    }

    public void ShowPanel()
    {
        if (!IsOnboarded)
        {
            panel.transform.localScale = Vector2.zero;

            panel?.SetActive(true);
            panel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            pointer.SetActive(false);
        }
    }
    public void HidePanel()
    {
        IsOnboarded = true;
        panel.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() => panel?.SetActive(false));
    }
}
