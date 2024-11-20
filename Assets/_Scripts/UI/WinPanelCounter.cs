using TMPro;
using UnityEngine;

public class WinPanelCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    private void OnEnable()
    {
        counterText.text = $"{ClothsController.Instance.ClothCount.Value}/{ClothsController.Instance.LevelClothsCount.Value}";
    }
}
