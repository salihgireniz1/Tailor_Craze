using R3;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static SerializableReactiveProperty<int> Amount { get; set; }
    private static string key = "Gold";

    private void Awake()
    {
        Amount = new SerializableReactiveProperty<int>(ES3.Load(key, 100));
    }

    public static void ChangeAmount(int change)
    {
        Amount.Value = Mathf.Min(Amount.Value + change, 0);
        ES3.Save(key, Amount.Value);
    }
}
