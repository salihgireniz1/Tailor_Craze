
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GoldManager : MonoSingleton<GoldManager>
{
    public static SerializableReactiveProperty<int> Amount { get; set; }
    private static string key = "Gold";

    protected override void Awake()
    {
        Amount = new SerializableReactiveProperty<int>(ES3.Load(key, 100));
    }
    [Button]
    public void AddGold()
    {
        ChangeAmount(20);
    }
    public static bool ChangeAmount(int change)
    {
        if (Amount.Value + change < 0)
        {
            return false;
        }
        Amount.Value = Mathf.Max(Amount.Value + change, 0);
        ES3.Save(key, Amount.Value);
        return true;
    }
}
