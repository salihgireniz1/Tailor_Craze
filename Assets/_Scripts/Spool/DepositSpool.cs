using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class DepositSpool : BaseSpool, IFillable
{
    [SerializeField] private bool _isFilled;
    [SerializeField] private Yarn myYarn;
    public YarnData filledYarnData;
    private void Awake()
    {
        myYarn = GetComponentInChildren<Yarn>();
    }
    [Button]
    public async UniTask Fill(YarnData data)
    {
        _inProgress = true;
        _isFilled = true;
        filledYarnData = data;
        myYarn.Tube.clipTo = 0;
        myYarn.Tube.color = data.color;
        await YarnController.Instance.Rolling(myYarn, RollType.Roll, this);
        _inProgress = false;
    }
    public bool CanBeFilled(YarnData data)
    {
        return _isFilled;
    }
    protected override void RemoveContent(int index)
    {
        myYarn.Tube.clipTo = 0f;
        _isFilled = false;
    }
    public bool IsFilled { get => _isFilled; }
    public IConnect Connector => myYarn;
}
