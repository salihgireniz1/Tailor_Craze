using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

public class OnboardingManager : MonoSingleton<OnboardingManager>
{
    public GameObject pointer;
    public GameObject panel;

    [Header("Hidden Yarn Onboarding")]
    [SerializeField]
    private GameObject _hiddenOnboardingPanel;
    [SerializeField]
    private GameObject _hiddenOnboardingContent;

    public bool IsOnboardingHiddenYarn = false;
    public bool IsOnboarded
    {
        get => ES3.Load("onboarded", false);
        set => ES3.Save("onboarded", value);
    }
    public bool IsOnboardedHidden
    {
        get => ES3.Load("onboardedHidden", false);
        set => ES3.Save("onboardedHidden", value);
    }
    DisposableBag bag = new DisposableBag();
    private void Start()
    {
        if (!IsOnboarded)
        {
            pointer.SetActive(true);
        }
        if (!IsOnboardedHidden)
        {
            GameManager.CurrentState
            .Where(state => state == GameState.Initializing && LevelManager.Level >= 10)
            .Subscribe(
                _ => ShowHiddenOnboarding().Forget()
            ).AddTo(ref bag);
        }
    }
    void OnDestroy()
    {
        bag.Dispose();
    }
    public void HidePointer()
    {
        if (!IsOnboarded) pointer?.SetActive(false);
    }
    public async UniTaskVoid ShowHiddenOnboarding()
    {
        IsOnboardingHiddenYarn = true;

        _hiddenOnboardingContent.transform.localScale = Vector3.zero;
        _hiddenOnboardingPanel?.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        _hiddenOnboardingContent.transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBack);

        IsOnboardedHidden = true;
        IsOnboardingHiddenYarn = false;
        bag.Clear();
    }
    public async void HideHiddenOnboardingPanel()
    {
        await _hiddenOnboardingContent.transform.DOScale(Vector3.zero, .25f).SetEase(Ease.InBack).ToUniTask();
        _hiddenOnboardingPanel?.SetActive(false);
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
