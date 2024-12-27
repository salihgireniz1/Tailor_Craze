using System.Collections;
using System.Collections.Generic;
using TailorCraze.Haptic;
using UnityEngine;

public class SettingsPopup : MonoBehaviour
{
    [SerializeField] ToggleButton musicButton;
    [SerializeField] ToggleButton soundButton;
    [SerializeField] ToggleButton vibrateButton;

    public void Open()
    {
        musicButton.SetActive(!SoundManager.Instance.isBgmMuted);
        soundButton.SetActive(!SoundManager.Instance.isSfxMuted);
        vibrateButton.SetActive(HapticManager.IsOnVibration);
    }

    public void OnMusicButtonClicked()
    {
        Settings.Instance.ToggleMusic();

        musicButton.SetActive(!SoundManager.Instance.isBgmMuted);
    }

    public void OnSfxButtonClicked()
    {
        Settings.Instance.ToggleSounds();

        soundButton.SetActive(!SoundManager.Instance.isSfxMuted);
    }

    public void OnVibrationButtonClicked()
    {
        Settings.Instance.ToggleHaptics();
        vibrateButton.SetActive(HapticManager.IsOnVibration);
    }
}
