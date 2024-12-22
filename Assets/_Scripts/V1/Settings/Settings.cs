using TailorCraze.Haptic;
using UnityEngine;

public class Settings : MonoSingleton<Settings>
{
    #region Inspector Fields

    [SerializeField] private KnittingSettings _knittingSettings;

    [SerializeField] private KnittingAnimationData _knittingAnimationData;

    [SerializeField] private BandAnimationData _bandAnimationData;

    [SerializeField] private SpoolAnimationSettings _spoolAnimationSettings;

    [SerializeField] private GameData _gameData;
    #endregion

    #region Properties

    public static KnittingSettings KnittingSettings => Instance._knittingSettings;

    public static KnittingAnimationData KnittingAnimationData => Instance._knittingAnimationData;

    public static BandAnimationData BandAnimationData => Instance._bandAnimationData;

    public static SpoolAnimationSettings SpoolAnimationSettings => Instance._spoolAnimationSettings;

    public static GameData GameData => Instance._gameData;

    #endregion

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
