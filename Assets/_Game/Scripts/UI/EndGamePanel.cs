using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using TMPro;
using UnityEngine;

public class EndGamePanel : BasePanel
{
    [SerializeField] GameObject winPanel, losePanel;
    [SerializeField] ParticleImage coinParticle;

    bool isTapToContinueClicked = false;

    public override void OnAppearStart()
    {
        base.OnAppearStart();
        if (GameManager.CurrentState.Value == GameState.Victory)
        {
            coinParticle.Stop();
            coinParticle.Play();
        }
        winPanel.SetActive(GameManager.CurrentState.Value == GameState.Victory);
        losePanel.SetActive(GameManager.CurrentState.Value == GameState.GameOver);
        isTapToContinueClicked = false;
    }

    public void OnTapToContinueClick()
    {
        if (!isTapToContinueClicked)
        {
            isTapToContinueClicked = true;

            UIManager.ShowLoading(() =>
            {
                GameManager.IsRestart = false;
                GameManager.CurrentState.Value = GameState.Initializing;
            });
        }
    }
}
