using R3;
using DG.Tweening;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class ClothsController : MonoSingleton<ClothsController>
{
    [SerializeField] Transform[] _spots;
    [SerializeField] Transform _spawnPoint;
    [SerializeField] Vector3 _offset = new Vector3(0, 1f, -0.5f);
    [SerializeField] Transform _clothParent;
    public List<FactoryCloth> activeCloths = new();
    private FactoryCloth[] _levelCloths;
    [SerializeField] private int _clothCount;
    private Dictionary<FactoryCloth, Transform> _clothSpotDict = new();
    public ReactiveProperty<int> ClothCount { get; private set; }
    public ReactiveProperty<int> LevelClothsCount { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        ClothCount = new(0);
        LevelClothsCount = new(0);
    }
    private void Start()
    {
        GameManager.CurrentState
        .Where(state => state == GameState.Initializing)
        .Subscribe(
            _ => InitLevelCloths()
        ).AddTo(this);
    }
    public UniTask CompleteCloth(FactoryCloth cloth)
    {
        if (_clothSpotDict.ContainsKey(cloth))
        {
            _clothSpotDict.Remove(cloth);
        }
        if (activeCloths.Contains(cloth))
        {
            activeCloths.Remove(cloth);
        }
        // Actually, earn some golds, do some animations etc.
        Destroy(cloth.gameObject);
        ClothCount.Value++;
        if (_clothCount >= _levelCloths.Length && activeCloths.Count <= 0)
        {
            Debug.Log("WIN!");
            GameManager.CurrentState.Value = GameState.Victory;
        }

        return UniTask.CompletedTask;
    }

    public ClothPart GetClothWithData(YarnData data)
    {
        for (int i = activeCloths.Count - 1; i >= 0; i--)
        {
            ClothPart tmpPart = activeCloths[i].GetFillablePart(data);
            if (tmpPart != null)
            {
                return tmpPart;
            }
        }
        return null;
    }

    [Button]
    public void InitLevelCloths()
    {
        _levelCloths = LevelManager.GetLevelData().LevelCloths;
        _clothCount = 0;
        ClothCount.Value = 0;
        LevelClothsCount.Value = _levelCloths.Length;
        AddNewClothAndShiftRight().Forget();
    }
    int GetSpotIndex(Transform spot)
    {
        return _spots.ToList().IndexOf(spot);
    }
    Transform GetSpot(int index)
    {
        return _spots[index];
    }
    bool IsSpotFilled(Transform spot)
    {
        return _clothSpotDict.ContainsValue(spot);
    }
    int GetMostRightSpotIndex(Transform currentSpot)
    {
        int currentSpotIndex = GetSpotIndex(currentSpot);

        // Find the first empty spot from the right
        return GetMostRightSpotIndex(currentSpotIndex);
    }
    int GetMostRightSpotIndex(int currentSpotIndex)
    {
        // Find the first empty spot from the right
        while (!IsSpotFilled(GetSpot(currentSpotIndex + 1)))
        {
            currentSpotIndex++;
        }
        return currentSpotIndex;
    }
    [Button]
    public async UniTask AddNewClothAndShiftRight()
    {
        GameManager.CurrentState.Value = GameState.InProgress;
        List<UniTask> shifts = new();
        // Check if adding a new cloth would exceed the spot limit
        if (activeCloths.Count >= _spots.Length)
        {
            Debug.Log("Level Failed: No more spots to shift right.");
            GameManager.CurrentState.Value = GameState.GameOver;
            return;
        }

        // Shift each cloth one spot to the right, starting from the last
        for (int i = activeCloths.Count - 1; i >= 0; i--)
        {
            var currentCloth = activeCloths[i];
            var currentSpot = _clothSpotDict[currentCloth];
            int nextSpotIndex = (i == activeCloths.Count - 1) ? GetSpotIndex(currentSpot) + 1 : GetMostRightSpotIndex(currentSpot);
            if (nextSpotIndex < _spots.Length)
            {
                _clothSpotDict[currentCloth] = _spots[nextSpotIndex];

                UniTask task = currentCloth.transform.DOMove(_spots[nextSpotIndex].position + _offset, 0.5f).SetEase(Ease.OutBack).ToUniTask();
                shifts.Add(task);
            }
            else
            {
                Debug.Log("Level Failed: No more spots to shift right.");
                GameManager.CurrentState.Value = GameState.GameOver;
                return;
            }
        }
        FactoryCloth newCloth = null;
        // If there are still cloths to spawn for this level,
        if (_clothCount < _levelCloths.Length)
        {
            // Spawn the new cloth at the left-most spot
            newCloth = Instantiate(_levelCloths[_clothCount], _spawnPoint.position + _offset, Quaternion.identity, _clothParent);
            int spawnAlignmentIndex = (activeCloths.Count == 0) ? 0 : GetMostRightSpotIndex(-1);
            UniTask alignToStart = newCloth.transform.DOMove(_spots[spawnAlignmentIndex].position + _offset, .5f).SetEase(Ease.OutBack).ToUniTask();
            shifts.Add(alignToStart);

            activeCloths.Insert(0, newCloth); // Add new cloth to the beginning of the list
            _clothSpotDict[newCloth] = _spots[spawnAlignmentIndex];
            _clothCount++;
        }

        await UniTask.WhenAll(shifts);
        // Await deposits to control new cloth.
        await DepositSpoolController.Instance.CheckNewClothAsync(newCloth);

        // Ensure that there are at least two cloths on the band.
        if (activeCloths.Count <= 1)
        {
            await AddNewClothAndShiftRight();
        }

        if (GameManager.CurrentState.Value == GameState.InProgress)
            GameManager.CurrentState.Value = GameState.Playing;
    }
}
