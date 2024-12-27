using UnityEngine;
using R3;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
public class DeskSpot : MonoBehaviour
{
    public Vector3 Position => _transform.position;
    public SpoolPlane ActivePlane
    {
        get => _activePlane;
        set
        {
            _activePlane = value;
        }
    }
    public bool IsLocked => _isLocked;
    public bool IsOccupied => ActivePlane != null;
    [SerializeField] private bool _lockedAsDefault;
    private Transform _transform;
    private bool _isLocked;
    private Collider _collider;
    private SpoolPlane _activePlane;

    [SerializeField] private GameObject locks;
    [SerializeField] private ParticleSystem _dust;

    private void Awake()
    {
        _transform = transform;
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        GameManager.CurrentState.Subscribe(state =>
        {
            if (state == GameState.Initializing)
            {
                if (_lockedAsDefault) Lock();
                else Unlock().Forget();
            }
        }).AddTo(this);

    }
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject() || !_isLocked) return;
        if (GameManager.CurrentState.Value != GameState.Playing) return;
        SelectSpot();
    }
    public void PlayDust()
    {
        _dust?.Stop();
        _dust?.Play();
    }
    public void SelectSpot()
    {
        if (GoldManager.ChangeAmount(-100))
        {
            Unlock().Forget();
        }
    }
    public async UniTaskVoid Unlock()
    {
        _isLocked = false;
        _collider.enabled = false;
        SoundManager.Instance.PlaySFX(SFXType.UnlockGrid);

        await locks.transform.DOScale(Vector2.zero, 0.4f).SetEase(Ease.InBack).ToUniTask();
        locks.SetActive(false);

    }


    public void Lock()
    {
        _isLocked = true;
        _collider.enabled = true;
        locks.transform.localScale = Vector3.one;
        locks.SetActive(true);
    }
}
