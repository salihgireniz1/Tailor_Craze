using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PopUpController : MonoBehaviour
{
    [SerializeField] private GameObject _countPart;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private float _remainDuration = 1.5f;

    [SerializeField] private TextMeshProUGUI _levelText;

    public async UniTaskVoid RevealPopUpAsync()
    {
        _countPart.transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

        _countText.text = ClothsController.Instance.LevelClothsCount.Value + "x";
        _levelText.text = "Level " + LevelManager.Level;

        // Activate popup.
        await _countPart.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();

        // Wait for remain duration.
        await UniTask.Delay(TimeSpan.FromSeconds(_remainDuration));

        // Deactivate popup.
        await _countPart.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        gameObject.SetActive(false);
    }

}
