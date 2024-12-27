using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image backgroundImage;
    [SerializeField] Sprite activeSprite;
    [SerializeField] Sprite inactiveSprite;

    bool _isActive;

    public Button Button => button;
        
    public void SetActive(bool active)
    {
        _isActive = active;
        backgroundImage.sprite = _isActive ? activeSprite : inactiveSprite;
    }
}