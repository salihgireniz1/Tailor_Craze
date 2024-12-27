
using R3;
using AssetKits.ParticleImage;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGamePanel : BasePanel
{
    [SerializeField] ParticleImage confettiParticle;
    [SerializeField] Image progressBarFillImage;
    [SerializeField] SettingsPopup settingsPanel;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] Image handImage;
    int totalManCount = 0;
    void OnEnable()
    {
        settingsPanel.gameObject.SetActive(false);
        progressBarFillImage.fillAmount = 0f;
        confettiParticle.gameObject.SetActive(false);
        totalManCount = LevelManager.CurrentLevel.MannequinLines.Length;
        DistributionManager.Instance.ProgressPercent.Subscribe(percent => OnCharacterCountChanged(percent)).AddTo(this);
        DeskManager.Instance.FirstSelection.Subscribe(v => { if (v) CloseTutorialPanel(); }).AddTo(this);
    }


    public override float Appear(float delay = 0)
    {
        levelText.text = $"Level {LevelManager.Level}";
        if (LevelManager.Level == 1)
        {
            OpenTutorialPanel(LevelManager.CurrentLevel.SpoolLines[0].PeekFirst().transform);
        }
        else
        {
            CloseTutorialPanel();
        }
        return base.Appear(delay);
    }

    public override float Disappear(float delay = 0)
    {
        if (GameManager.CurrentState.Value == GameState.Victory)
        {
            confettiParticle.gameObject.SetActive(true);
            confettiParticle.Play();
        }
        return base.Disappear(delay);
    }

    public void OpenTutorialPanel(Transform trayPos)
    {
        tutorialPanel.SetActive(true);
        HandOnPos(trayPos.position);
    }

    public void HandOnPos(Vector3 pos)
    {
        handImage.enabled = true;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);

        if (screenPos.z > 0)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                handImage.canvas.transform as RectTransform,
                screenPos,
                handImage.canvas.worldCamera,
                out Vector2 uiPos
            );

            handImage.rectTransform.localPosition = uiPos;

            HandAnimation();
        }
        else
        {
            handImage.enabled = false;
        }
    }


    void HandAnimation()
    {
        handImage.rectTransform.localScale = Vector3.zero;
        DOTween.Sequence()
            .AppendInterval(.3f)
            .Append(handImage.rectTransform.DOScale(new Vector3(-1f, 1f, 1f), .8f)
                .SetEase(Ease.OutBack))
            .OnComplete(() =>
            {
                handImage.rectTransform.DOScale(handImage.rectTransform.localScale * 1.1f, .8f)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Yoyo);
            });
    }

    public void CloseTutorialPanel()
    {
        tutorialPanel.SetActive(false);
    }

    Tween progressTween;
    void OnCharacterCountChanged(float amount)
    {
        progressTween?.Kill();

        progressTween = progressBarFillImage.DOFillAmount(amount, .2f).SetEase(Ease.Linear);
    }

    public void OnRestartButtonClicked()
    {
        GameManager.IsRestart = true;
        UIManager.ShowLoading(() =>
        {
            GameManager.CurrentState.Value = GameState.Initializing;
        });
    }

    public void OnSettingsButtonClicked()
    {
        settingsPanel.gameObject.SetActive(true);
        settingsPanel.Open();
    }

    public void OnSettingsCloseButtonClicked()
    {
        settingsPanel.gameObject.SetActive(false);
    }
}
