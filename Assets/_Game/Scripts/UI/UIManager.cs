using R3;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] LoadingBlackout loadingBlackout;
    [SerializeField] List<BasePanel> allPanels;

    List<BasePanel> activePanels = new List<BasePanel>();


    new void Awake()
    {
        base.Awake();
        GameManager.CurrentState.Subscribe(state =>
        {
            SwitchUI(state);
            switch (state)
            {
                case GameState.MainMenu:
                case GameState.Playing:
                    HideLoading();
                    break;
                default:
                    break;
            }
        })
        .AddTo(this);
    }

    void SwitchUI(GameState state)
    {
        var delay = 0f;

        for (int i = activePanels.Count - 1; i >= 0; i--)
        {
            var panel = activePanels[i];
            if (!panel.appearStates.Contains(state))
            {
                delay = Mathf.Max(delay, panel.Disappear());
                activePanels.RemoveAt(i);
            }
        }

        var panelsToOpen = allPanels.Where(o => o.appearStates.Contains(state)).Except(activePanels);
        foreach (var panel in panelsToOpen)
        {
            panel.Appear(delay);
        }
        activePanels.AddRange(panelsToOpen);
    }

    public static void ShowLoading(UnityAction onComplete = null, bool isInstant = false)
    {
        Instance.loadingBlackout.FadeIn(onComplete, isInstant);
    }

    public static void HideLoading(UnityAction onComplete = null, bool isInstant = false)
    {
        Instance.loadingBlackout.FadeOut(onComplete, isInstant);
    }
}