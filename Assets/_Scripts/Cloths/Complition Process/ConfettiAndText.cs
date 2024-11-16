
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ConfettiAndText : MonoBehaviour, IClothComplition
{
    [SerializeField] private GameObject _awesomeText;
    [SerializeField] private GameObject _confetti;
    [SerializeField] private float _waitDuration;
    public async UniTask Complete()
    {
        _confetti?.SetActive(true);
        _awesomeText?.SetActive(true);
        _awesomeText.transform.DOScale(1f, .3f).SetEase(Ease.OutBack);
        await UniTask.Delay(TimeSpan.FromSeconds(_waitDuration));
        _confetti?.SetActive(false);
        _awesomeText?.SetActive(false);
    }
}
