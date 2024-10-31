using TailorCraze.Haptic;
using UnityEngine;

public class Settings : MonoSingleton<Settings>
{
    [field: SerializeField] public KnittingSettings KnittingSettings { get; private set; }

    public void ToggleHaptics()
    {
        HapticManager.IsOnVibration = !HapticManager.IsOnVibration;
    }
    public void ToggleMusic()
    {

    }
    public void ToggleSounds()
    {

    }
}
