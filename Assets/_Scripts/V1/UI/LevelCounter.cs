using R3;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class LevelCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _levelText;
    private void Awake()
    {
        if (!_levelText) _levelText = GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        LevelManager.LevelProperty.Subscribe(
            level => _levelText.text = $"Level {level}"
        ).AddTo(this);
    }
}
