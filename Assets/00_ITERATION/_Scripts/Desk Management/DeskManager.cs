using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class DeskManager : MonoSingleton<DeskManager>
{
    [Title("Plane Positioning Settings")]
    [SerializeField]
    private float _jumpPower = 2F;

    [SerializeField]
    private float _jumpDuration = 0.5F;

    [SerializeField]
    private float _positionYOffset = 0.1f;

    [Title("Desk Spots")]

    [SerializeField]
    private DeskSpot[] _spots;
    CancellationTokenSource cts;

    public async UniTask CheckSpotPlanes()
    {
        cts?.Cancel();
        cts = new();
        var sortedSpots = _spots.OrderBy(spot => spot.ActivePlane?.Fillness ?? 0).ToArray();

        for (int i = 0; i < sortedSpots.Length; i++)
        {
            if (sortedSpots[i].IsOccupied && !sortedSpots[i].IsLocked)
            {
                if (cts.IsCancellationRequested) return;
                await DistributionManager.Instance.Distribute(sortedSpots[i].ActivePlane).AttachExternalCancellation(cts.Token);
            }
        }

        await UniTask.Delay(1000, cancellationToken: cts.Token);
        if (cts.IsCancellationRequested) return;

        if (LineManager.Instance.AllMannequinsCleared())
        {
            GameManager.CurrentState.Value = GameState.Victory;
            cts.Cancel();
            return;
        }

        var firstSpot = FirstAvailableSpot();
        if (firstSpot == default)
        {
            Debug.Log("GAME FAILED.");
            GameManager.CurrentState.Value = GameState.GameOver;
            cts.Cancel();
        }
    }

    public DeskSpot FirstAvailableSpot()
    {
        for (int i = 0; i < _spots.Length; i++)
        {
            if (!_spots[i].IsOccupied && !_spots[i].IsLocked)
            {
                return _spots[i];
            }
        }
        return default;
    }
    public async UniTask FillSpot(SpoolPlane plane)
    {
        var availableSpot = FirstAvailableSpot();
        if (availableSpot != default)
        {
            availableSpot.ActivePlane = plane;
            var spotPosition = availableSpot.Position + Vector3.up * _positionYOffset;
            plane.ControlTubeActivition(false);
            await plane.transform.DOJump(spotPosition, _jumpPower, 1, _jumpDuration).SetEase(Ease.OutFlash).ToUniTask();
            SoundManager.Instance.PlaySFX(SFXType.TrayMove);
            availableSpot.PlayDust();
            await plane.transform.DOPunchScale(Vector3.down * 0.4f, 0.25f, 1).ToUniTask();
            plane.ControlTubeActivition(true);
            await CheckSpotPlanes();
        }
    }
}
