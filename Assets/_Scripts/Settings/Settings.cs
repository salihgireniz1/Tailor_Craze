using UnityEngine;

public class Settings : MonoSingleton<Settings>
{
    [field: SerializeField] public KnittingSettings KnittingSettings { get; private set; }
}
