using R3;
using TMPro;
using UnityEngine;

public class ClothProgressCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    private int _current;
    private int _total;
    void Start()
    {
        counterText.text = $"{ClothsController.Instance.ClothCount.Value}/{ClothsController.Instance.LevelClothsCount.Value}";

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
