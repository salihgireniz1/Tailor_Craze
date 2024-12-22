using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Mannequin : MonoBehaviour, IQueueable<Mannequin>
{
    [SerializeField]
    private Transform _currentStandPoint;

    [SerializeField]
    private FactoryCloth _factoryCloth;

    [SerializeField]
    private BaseLine<Mannequin> _currentLine;


    private void OnValidate()
    {
        if (!_factoryCloth) TryGetComponent(out _factoryCloth);
    }

    public async UniTask AlignToPoint(Transform newPoint, float duration = 0)
    {
        if (duration == 0f)
        {
            transform.position = newPoint.position;
            transform.LookAt(newPoint);
        }
        else
        {
            transform.DOLookAt(newPoint.position, .1f).ToUniTask().Forget();
            await Move(newPoint.position, duration);
        }
        CurrentStandPoint = newPoint;
    }

    public async UniTask Move(Vector3 position, float duration = 0)
    {
        await transform.DOMove(position, duration).ToUniTask();
        FactoryCloth.AdjustmentShakeAnim();
    }

    public BaseLine<Mannequin> CurrentLine { get => _currentLine; set => _currentLine = value; }

    public Transform CurrentStandPoint
    {
        get => _currentStandPoint;
        set
        {
            _currentStandPoint = value;
            transform.SetParent(_currentStandPoint);
        }
    }
    public FactoryCloth FactoryCloth => _factoryCloth;
    public bool InProgress { get; set; }
}
