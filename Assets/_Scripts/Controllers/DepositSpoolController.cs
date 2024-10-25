using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DepositSpoolController : MonoSingleton<DepositSpoolController>
{
    public DepositSpool[] _depositSpools;

    public bool HasEmptyDepositSpool => _depositSpools.Any(x => !x.IsFilled);
    public DepositSpool FirstEmptyDepositSpool => _depositSpools.FirstOrDefault(x => !x.IsFilled);

    public async UniTaskVoid FillDeposit(YarnData data)
    {
        if (!HasEmptyDepositSpool)
        {
            Debug.LogWarning("There is no deposit. Destroying all deposit yarns.");
            return;
        }
        await FirstEmptyDepositSpool.Fill(data);
    }
}