using R3;
using TMPro;
using UnityEngine;

public class ClothProgressCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    private int _current;
    private int _total;
    private void OnEnable()
    {
        _current = ClothsController.Instance.ClothCount.Value;
        _total = ClothsController.Instance.LevelClothsCount.Value;
        counterText.text = $"{_current}/{_total}";
    }
    void Start()
    {
        ClothsController.Instance.ClothCount
        .Subscribe(c =>
        {
            _current = c;
            counterText.text = $"{_current}/{_total}";
        }
        ).AddTo(this);

        ClothsController.Instance.LevelClothsCount
        .Subscribe(c =>
        {
            _total = c;
            _current = 0;
            counterText.text = $"{_current}/{_total}";
        }).AddTo(this);
    }
}
