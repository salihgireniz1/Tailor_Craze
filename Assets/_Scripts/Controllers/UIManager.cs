using R3;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoSingleton<UIManager>
{
    public GameObject WinPanel;
    public GameObject LosePanel;

    public GameObject likeEmoji;
    public GameObject sadEmoji;

    private void Start()
    {
        HidePanels();

        GameManager.CurrentState.Subscribe(
            state =>
            {
                switch (state)
                {
                    case GameState.Victory:
                        WinPanel.SetActive(true);
                        likeEmoji?.SetActive(true);
                        sadEmoji.transform.DOScale(1f, .75f).SetEase(Ease.OutBack);
                        break;
                    case GameState.GameOver:
                        LosePanel.SetActive(true);
                        sadEmoji?.SetActive(true);
                        sadEmoji.transform.DOScale(1f, .75f).SetEase(Ease.OutBack);
                        break;
                    default:
                        HidePanels();
                        break;
                }
            }
        ).AddTo(this);
    }
    public void HidePanels()
    {
        WinPanel?.SetActive(false);
        LosePanel?.SetActive(false);

        likeEmoji?.SetActive(false);
        sadEmoji?.SetActive(false);

        likeEmoji.transform.localScale = Vector3.zero;
        sadEmoji.transform.localScale = Vector3.zero;
    }

    public void ReloadScene()
    {
        // GameManager.CurrentState.Value = GameState.Initializing;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
