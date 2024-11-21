using R3;
using System;
using DG.Tweening;
using UnityEngine;
using System.Threading;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class SpoolController : MonoSingleton<SpoolController>
{
    [SerializeField] private Transform _spoolsParent;
    [SerializeField] private GameObject[] spoolPoints;
    [SerializeField] private Spool _spoolPrefab;
    [SerializeField] private List<Spool> _activeSpools = new();

    public SpoolInfo[] _levelSpools;

    private int _spoolCount = 0;
    private CancellationTokenSource cts = new();
    private IRandomSpool _randomizer;
    public bool waitingPopUp = true;
    private async void Start()
    {
        waitingPopUp = true;
        _randomizer = new GetRandomForExistingCloths();
        GameManager.CurrentState
        .Where(state => state == GameState.Initializing)
        .Subscribe(
            _ => InitLevelSpools()
        ).AddTo(this);

        await UniTask.Delay(3000);
        waitingPopUp = false;
    }
    [Button]
    private void FindPoints()
    {
        spoolPoints = GameObject.FindGameObjectsWithTag("Spool Point");
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
        await spool.transform
            .DOScale(Vector3.zero, Settings.SpoolAnimationSettings.SpoolUnscaleDuration)
            .SetEase(Settings.SpoolAnimationSettings.SpoolUnscaleEase)
            .WithCancellation(cts.Token);

        if (_spoolCount >= _levelSpools.Length && !_canRandom)
        {
            return;
        }
        await UniTask.Delay(TimeSpan.FromSeconds(Settings.SpoolAnimationSettings.DelayBetweenOldAndNewSpool));
        var targetPos = spool.transform.position;

        Spool newSpool = SpawnSpool(GetNextSpoolOfLevel(), targetPos);

        var defaultScale = newSpool.transform.localScale;
        newSpool.transform.localScale = defaultScale * Settings.SpoolAnimationSettings.NewSpoolStartScale;
        DestroyImmediate(spool.gameObject);

        var scaleSpool = newSpool.transform
            .DOScale(defaultScale, Settings.SpoolAnimationSettings.SpoolScaleDuration)
            .SetEase(Settings.SpoolAnimationSettings.SpoolScaleEase)
            .WithCancellation(cts.Token);

        await scaleSpool;
    }
    public SpoolInfo GetNextSpoolOfLevel()
    {
        int index = _spoolCount < _levelSpools.Length ? _spoolCount : -1;
        if (index == -1) return _randomizer.GetRandomSpoolInfo();
        return _levelSpools[index];
    }
    SpoolInfo GetMixed()
    {
        return _randomizer.GetRandomSpoolInfo();
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
        for (int i = 0; i < _levelSpools.Length; i++)
        {
            _activeSpools.Add(SpawnSpool(_levelSpools[i], spoolPoints[i].transform.position));
        }
    }
}
