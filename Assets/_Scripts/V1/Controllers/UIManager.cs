using R3;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoSingleton<UIManager>
{
    public GameObject InGamePanel;
    public GameObject WinPanel;
    public GameObject LosePanel;

    public GameObject likeEmoji;
    public GameObject sadEmoji;

    public GameObject LoadingPanel;

    private void Start()
    {
        HidePanels();

        GameManager.CurrentState.Subscribe(
            state =>
            {
                switch (state)
                {
                    case GameState.Victory:
                        InGamePanel.SetActive(false);
                        WinPanel.SetActive(true);
                        likeEmoji?.SetActive(true);
                        sadEmoji.transform.DOScale(1f, .75f).SetEase(Ease.OutBack);
                        break;
                    case GameState.GameOver:
                        InGamePanel.SetActive(false);
                        LosePanel.SetActive(true);
                        sadEmoji?.SetActive(true);
                        sadEmoji.transform.DOScale(1f, .75f).SetEase(Ease.OutBack);
                        break;
                    case GameState.Initializing:
                        HidePanels();
                        LoadingPanel?.SetActive(true);
                        break;
                    default:
                        LoadingPanel?.SetActive(false);
                        break;
                }
            }
        ).AddTo(this);
    }
    public void HidePanels()
    {
        InGamePanel.SetActive(true);

        WinPanel?.SetActive(false);
        LosePanel?.SetActive(false);

        likeEmoji?.SetActive(false);
        sadEmoji?.SetActive(false);

        likeEmoji.transform.localScale = Vector3.zero;
        sadEmoji.transform.localScale = Vector3.zero;
    }

    public void ReloadScene()
    {
        GameManager.CurrentState.Value = GameState.Initializing;
    }
    public void NextScene()
    {
        GameManager.CurrentState.Value = GameState.Initializing;
    }
}
