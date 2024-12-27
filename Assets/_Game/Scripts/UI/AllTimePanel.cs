using R3;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllTimePanel : BasePanel
{
    [SerializeField] TextMeshProUGUI moneyText;

    void Start()
    {
        GoldManager.Amount.Subscribe(amount =>
        {
            UpdateMoneyText(amount);
        }).AddTo(this);
    }

    public override float Appear(float delay = 0)
    {
        UpdateMoneyText(GoldManager.Amount.Value);
        return base.Appear(delay);
    }

    void UpdateMoneyText(int value)
    {
        moneyText.text = $"{value}";
    }
}
