using System.Linq;
using Cysharp.Threading.Tasks;

public class DepositSpoolController : MonoSingleton<DepositSpoolController>
{
    public DepositSpool[] _depositSpools;

    public bool HasEmptyDepositSpool => _depositSpools.Any(x => !x.IsFilled);
    public DepositSpool FirstEmptyDepositSpool => _depositSpools.FirstOrDefault(x => !x.IsFilled);

    public async UniTask HandleOverloadingAsync()
    {
        UniTask[] bursts = new UniTask[_depositSpools.Length];
        for (int i = 0; i < _depositSpools.Length; i++)
        {
            bursts[i] = _depositSpools[i].BurstContentAsync();
        }
        await UniTask.WhenAll(bursts);

        // foreach (var depositSpool in _depositSpools)
        // {
        //     await depositSpool.BurstContentAsync();
        // }
    }
    public async UniTask CheckNewClothAsync(FactoryCloth cloth)
    {
        if (cloth == null) return;
        foreach (var deposit in _depositSpools)
        {
            if (!deposit.IsFilled) continue;
            var fillablePart = cloth.GetFillablePart(deposit.filledYarnData);
            if (fillablePart == null) continue;

            YarnConnection.Instance.SetConnectionPoints(deposit.Connector, fillablePart.Connector);
            YarnConnection.Instance.ActivateConnection(deposit.filledYarnData).Forget();
            var duration = fillablePart.FillDuration;
            fillablePart.Fill(deposit.filledYarnData).Forget();
            await deposit.UnrollTopYarn(duration);
            await YarnConnection.Instance.BreakConnection();
        }
    }
}