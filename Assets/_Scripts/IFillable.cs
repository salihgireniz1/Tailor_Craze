
using Cysharp.Threading.Tasks;

public interface IFillable
{
    public bool IsFilled { get; }
    public bool CanBeFilled(YarnData data);
    public UniTask Fill(YarnData data);
    public IConnect Connector { get; }
    public float FillDuration { get; }
}
