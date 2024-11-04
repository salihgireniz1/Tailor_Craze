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
        var newSpool = SpawnSpool(GetNextSpoolOfLevel(), spool.transform.position);
        var defaultScale = newSpool.transform.localScale;
        newSpool.transform.localScale = Vector3.zero;
        DestroyImmediate(spool.gameObject);
        await newSpool.transform.DOScale(defaultScale, .5f).SetEase(Ease.OutBack).WithCancellation(cts.Token);
    }
    public SpoolInfo GetNextSpoolOfLevel()
    {
        return _levelSpools[_spoolCount % _levelSpools.Length];
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
    [Button]
    public void InitLevelSpools()
    {
        var data = LevelManager.GetLevelData();
        _levelSpools = data.LevelSpools;
        _spoolCount = 0;
        for (int i = 0; i < data.StartSpoolCount; i++)
        {
            _activeSpools.Add(SpawnSpool(_levelSpools[i], spoolPoints[i].transform.position));
        }
    }
}
