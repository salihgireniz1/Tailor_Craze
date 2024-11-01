using R3;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoSingleton<UIManager>
{
    public GameObject WinPanel;
    public GameObject LosePanel;
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
                        break;
                    case GameState.GameOver:
                        LosePanel.SetActive(true);
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
    }

    public void ReloadScene()
    {
        GameManager.CurrentState.Value = GameState.Initializing;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
