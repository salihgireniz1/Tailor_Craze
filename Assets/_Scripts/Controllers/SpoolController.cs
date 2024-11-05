using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpoolController : MonoSingleton<SpoolController>
{
    [SerializeField] private Transform _spoolsParent;
    [SerializeField] private GameObject[] spoolPoints;
    [Button]
    private void FindPoints()
    {
        spoolPoints = GameObject.FindGameObjectsWithTag("Spool Point");
    }
    [SerializeField] private Spool _spoolPrefab;
    [SerializeField] private List<Spool> _activeSpools = new();
    [SerializeField] SpoolInfo[] _levelSpools;
    private int _spoolCount = 0;
    private CancellationTokenSource cts = new();
    private void Start()
    {
        GameManager.CurrentState
        .Where(state => state == GameState.Initializing)
        .Subscribe(
            _ => InitLevelSpools()
        ).AddTo(this);
    }

    [Button]
    private void ClearActives()
    {
        foreach (var spool in _activeSpools)
        {
            DestroyImmediate(spool.gameObject);
        }
        _activeSpools = new();
        _levelSpools = default;
    }
    public async UniTask RemoveSpool(Spool spool)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        _activeSpools.Remove(spool);
        await spool.transform.DOScale(Vector3.zero, .2f).SetEase(Ease.InBack).WithCancellation(cts.Token);

        if (_spoolCount >= _levelSpools.Length && !_canRandom)
        {
            return;
        }

        var newSpool = SpawnSpool(GetNextSpoolOfLevel(), spool.transform.position);
        var defaultScale = newSpool.transform.localScale;
        newSpool.transform.localScale = Vector3.zero;
        DestroyImmediate(spool.gameObject);
        await newSpool.transform.DOScale(defaultScale, .5f).SetEase(Ease.OutBack).WithCancellation(cts.Token);
    }
    public SpoolInfo GetNextSpoolOfLevel()
    {
        int index = _spoolCount < _levelSpools.Length ? _spoolCount : Random.Range(0, _levelSpools.Length - 1);
        return _levelSpools[index];
    }
    public Spool SpawnSpool(SpoolInfo info, Vector3 point)
    {
        Spool instance = Instantiate(_spoolPrefab, point, Quaternion.identity, _spoolsParent);
        if (info.isRandom)
        {
            // int rndYarnCount = Random.Range(1, 4); // Spawn random amount of yarn at this spool min:1 max:3
            for (int i = 0; i < 3; i++)
            {
                YarnType rndType = YarnController.Instance.GetRandomYarnType();
                instance.AddContent(rndType, false);
            }
        }
        else
        {
            foreach (SpoolYarn yarns in info.Yarns)
            {
                instance.AddContent(yarns.type, yarns.isHidden);
            }
        }
        _spoolCount++;
        return instance;
    }
    bool _canRandom;
    [Button]
    public void InitLevelSpools()
    {
        LevelData data = LevelManager.GetLevelData();
        _canRandom = data.RandomNextSpool;
        _levelSpools = data.LevelSpools;
        _spoolCount = 0;
        for (int i = 0; i < data.StartSpoolCount; i++)
        {
            _activeSpools.Add(SpawnSpool(_levelSpools[i], spoolPoints[i].transform.position));
        }
    }
    // [Button]
    // public void InitLevelSpools()
    // {
    //     LevelData data = LevelManager.GetLevelData();
    //     _canRandom = data.RandomNextSpool;
    //     _levelSpools = data.LevelSpools;
    //     _spoolCount = 0;

    //     // Define your bounds and minimum offset here (or pass them as parameters)
    //     Vector2 spawnBoundsMin = new Vector2(-1.92f, -4.25f); // Example min bounds
    //     Vector2 spawnBoundsMax = new Vector2(1.92f, 0);   // Example max bounds
    //     float minOffset = 1.5f;  // Minimum distance required between spools

    //     for (int i = 0; i < _levelSpools.Length; i++)
    //     {
    //         Vector3 spawnPosition;
    //         bool validPosition;

    //         // Try finding a valid position
    //         do
    //         {
    //             spawnPosition = GetRandomPositionWithinBounds(spawnBoundsMin, spawnBoundsMax);
    //             validPosition = IsPositionValid(spawnPosition, minOffset);
    //         }
    //         while (!validPosition);

    //         // Spawn spool at the valid position
    //         _activeSpools.Add(SpawnSpool(_levelSpools[i], spawnPosition));
    //     }
    // }

    private Vector3 GetRandomPositionWithinBounds(Vector2 minBounds, Vector2 maxBounds)
    {
        float x = Random.Range(minBounds.x, maxBounds.x);
        float z = Random.Range(minBounds.y, maxBounds.y); // Assuming you want random positions in the XZ plane
        return new Vector3(x, 0, z); // Set Y to 0 or any desired height
    }

    private bool IsPositionValid(Vector3 position, float minOffset)
    {
        foreach (var spool in _activeSpools)
        {
            float distance = Vector3.Distance(position, spool.transform.position);
            float requiredOffset = minOffset + spool.transform.localScale.magnitude / 2; // Adjust based on spool size
            if (distance < requiredOffset)
            {
                return false; // Position is too close to an existing spool
            }
        }
        return true; // Position is valid
    }
}
