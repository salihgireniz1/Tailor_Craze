using TailorCraze.Haptic;
using UnityEngine;

public class Settings : MonoSingleton<Settings>
{
    [SerializeField] private KnittingSettings _knittingSettings;
    [SerializeField] private KnittingAnimationData _knittingAnimationData;
    [SerializeField] private BandAnimationData _bandAnimationData;
    public static KnittingSettings KnittingSettings => Instance._knittingSettings;
    public static KnittingAnimationData KnittingAnimationData => Instance._knittingAnimationData;
    public static BandAnimationData BandAnimationData => Instance._bandAnimationData;

    public void ToggleHaptics()
    {
        HapticManager.IsOnVibration = !HapticManager.IsOnVibration;
    }
    public void ToggleMusic()
    {
        SoundManager.Instance.ToggleMuteBGM();
    }
    public void ToggleSounds()
    {
        SoundManager.Instance.ToggleMuteSFX();
    }
}
