using Cysharp.Threading.Tasks;

public interface ISelectable
{
    public bool CanBeSelected { get; set; }
    public UniTask Select();
}
