using DG.Tweening;
using TMPro;
using UnityEngine;

public class MainMenuPanel : BasePanel
{
    [SerializeField] TextMeshProUGUI levelText, levelTextNext1, levelTextNext2, levelTextNext3;

    [SerializeField] TextMeshProUGUI leaderBoardSoonText, shopSoonText;

    public override void OnAppearStart()
    {
        base.OnAppearStart();
        leaderBoardSoonText.enabled = false;
        shopSoonText.enabled = false;
        levelText.text = $"{LevelManager.Level}";
        levelTextNext1.text = $"{LevelManager.Level + 1}";
        levelTextNext2.text = $"{LevelManager.Level + 2}";
        levelTextNext3.text = $"{LevelManager.Level + 3}";
    }

    public void StartButton()
    {
        // GameManager.Instance.GameStarted();
        GameManager.CurrentState.Value = GameState.Playing;
    }

    public void OnLeaderBoardButtonClicked()
    {
        PlayLeaderBoardSoonText();
    }

    public void OnShopButtonClicked()
    {
        PlayShopSoonText();
    }

    Tween leaderBoardTween, shopTween;

    void PlayLeaderBoardSoonText()
    {
        leaderBoardTween?.Kill();
        leaderBoardSoonText.rectTransform.localPosition = new Vector3(0f, 0f, 0f);

        leaderBoardSoonText.enabled = true;
        leaderBoardTween = leaderBoardSoonText.rectTransform.DOLocalMoveY(250f, .7f).SetEase(Ease.Linear)
            .OnComplete(() => leaderBoardSoonText.enabled = false);
    }

    void PlayShopSoonText()
    {
        shopTween?.Kill();
        shopSoonText.rectTransform.localPosition = new Vector3(0f, 0f, 0f);

        shopSoonText.enabled = true;
        shopSoonText.rectTransform.DOLocalMoveY(250f, .7f).SetEase(Ease.Linear)
            .OnComplete(() => shopSoonText.enabled = false);
    }
}