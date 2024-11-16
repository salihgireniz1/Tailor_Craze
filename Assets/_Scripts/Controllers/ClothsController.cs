using R3;
using DG.Tweening;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class ClothsController : MonoSingleton<ClothsController>
{
    [SerializeField] Transform[] _spots;
    [SerializeField] Transform _spawnPoint;
    // [SerializeField] Vector3 _offset = new Vector3(0, 1f, -0.5f);
    [SerializeField] Transform _clothParent;
    [SerializeField] Animation _anim;
    public List<FactoryCloth> activeCloths = new();
    private FactoryCloth[] _levelCloths;
    private int _clothCount;
    private Dictionary<FactoryCloth, Transform> _clothSpotDict = new();
    public ReactiveProperty<int> ClothCount { get; private set; }
    public ReactiveProperty<int> LevelClothsCount { get; private set; }
    CancellationDisposable cancellationDisposable = new();
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
    public override void OnDestroy()
    {
        base.OnDestroy();
        cancellationDisposable.Dispose();
    }

    #region Cloth Completion

    public async UniTask CompleteCloth(FactoryCloth cloth)
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

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// Clears all completed cloths from the active cloths list and destroys their game objects.
    /// </summary>
    /// <returns>A UniTask that completes when all completed cloths have been cleared.</returns>
    public async UniTask ClearCompletedCLoths()
    {
        // Initialize a list to store completed cloths.
        List<FactoryCloth> completedCloths = new();

        // Iterate through the active cloths list.
        foreach (var cloth in activeCloths)
        {
            // If the cloth is completed, add it to the completedCloths list.
            if (cloth.IsCompleted()) completedCloths.Add(cloth);
        }

        // Initialize an array to store the completion animations of the completed cloths.
        UniTask[] completeAnimations = new UniTask[completedCloths.Count];

        // Iterate through the completed cloths list.
        for (int i = 0; i < completedCloths.Count; i++)
        {
            // Start the completion animation for each completed cloth and store the UniTask in the completeAnimations array.
            completeAnimations[i] = CompleteCloth(completedCloths[i]);
        }

        // Wait for all completion animations to complete.
        await UniTask.WhenAll(completeAnimations);

        // Mark game as victory if every cloth in level completed.
        if (_clothCount >= _levelCloths.Length && activeCloths.Count <= 0)
        {
            GameManager.CurrentState.Value = GameState.Victory;
        }
    }
    #endregion

    #region Cloth Data Management
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
    public void InitLevelCloths()
    {
        _levelCloths = LevelManager.GetLevelData().LevelCloths;
        activeCloths = new();
        _clothCount = 0;
        ClothCount.Value = 0;
        LevelClothsCount.Value = _levelCloths.Length;
        AddNewClothAndShiftRight().AttachExternalCancellation(cancellationDisposable.Token).Forget();
    }
    #endregion

    #region Helper Methods
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
    #endregion

    #region Core Loop Methods
    public async UniTask AddNewClothAndShiftRight()
    {
        if (GameManager.CurrentState.Value == GameState.GameOver || GameManager.CurrentState.Value == GameState.Victory) return;
        GameManager.CurrentState.Value = GameState.InProgress;
        List<UniTask> shifts = new();
        // Check if adding a new cloth would exceed the spot limit
        if (activeCloths.Count >= _spots.Length)
        {
            GameManager.CurrentState.Value = GameState.GameOver;
            return;
        }

        // Shift each cloth one spot to the right, starting from the last
        for (int i = activeCloths.Count - 1; i >= 0; i--)
        {
            FactoryCloth currentCloth = activeCloths[i];
            if (currentCloth == null)
            {
                return;
            }
            var currentSpot = _clothSpotDict[currentCloth];
            int nextSpotIndex = (i == activeCloths.Count - 1) ? GetSpotIndex(currentSpot) + 1 : GetMostRightSpotIndex(currentSpot);
            if (nextSpotIndex < _spots.Length)
            {
                _clothSpotDict[currentCloth] = _spots[nextSpotIndex];

                UniTask task = currentCloth.transform
                    .DOMove(_spots[nextSpotIndex].position + Settings.BandAnimationData.spotOffset, Settings.BandAnimationData.shiftingDuration)
                    .SetEase(Settings.BandAnimationData.shiftingEase)
                    .OnComplete(() => currentCloth.AdjustmentShakeAnim())
                    .WithCancellation(UniTaskCancellationExtensions.GetCancellationTokenOnDestroy(currentCloth.gameObject));

                shifts.Add(task);
            }
            else
            {
                GameManager.CurrentState.Value = GameState.GameOver;
                return;
            }
        }

        // Check if any cloth came to the last spot.
        foreach (var pair in _clothSpotDict)
        {
            if (GetSpotIndex(pair.Value) == _spots.Length - 1)
            {
                _anim.enabled = true;
                break;
            }

            _anim.enabled = false;
        }

        await UniTask.WhenAll(shifts.Concat(FillGapsWithCloth()));


        // Ensure that there are at least two cloths on the band.
        if (activeCloths.Count == 1 && _clothCount < _levelCloths.Length)
        {
            await AddNewClothAndShiftRight().AttachExternalCancellation(cancellationDisposable.Token);
        }

        for (int i = activeCloths.Count - 1; i >= 0; i--)
        {
            // Await deposits to control new cloth.
            await DepositSpoolController.Instance.CheckNewClothAsync(activeCloths[i]);
            await ClearCompletedCLoths();
        }

        if (GameManager.CurrentState.Value == GameState.InProgress)
            GameManager.CurrentState.Value = GameState.Playing;
    }

    /// <summary>
    /// Fills any gaps in the cloth band by spawning new cloths and aligning them to the correct spots.
    /// </summary>
    /// <returns>A list of UniTasks representing the movement animations of the spawned cloths.</returns>
    List<UniTask> FillGapsWithCloth()
    {
        List<UniTask> spawnTasks = new();

        FactoryCloth newCloth;
        bool isFirst = activeCloths.Count == 0;
        // Determine the index of the last occupied spot on the band.
        int spawnAlignmentIndex = isFirst ? 0 : GetMostRightSpotIndex(-1);

        // Iterate through the spots from the last occupied spot to the first.
        for (int i = spawnAlignmentIndex; i >= 0; i--)
        {
            if (_clothCount >= _levelCloths.Length) break;

            // Calculate the spawn position for the new cloth.
            var spawnPos = new Vector3(
            _spawnPoint.position.x - (2f * (spawnAlignmentIndex - i)),
            _spawnPoint.position.y,
            _spawnPoint.position.z);

            // Instantiate a new cloth at the calculated spawn position.
            newCloth = Instantiate(
                _levelCloths[_clothCount],
                spawnPos + Settings.BandAnimationData.spotOffset,
                Quaternion.identity,
                _clothParent);

            // Add the new cloth to the beginning of the active cloths list.
            activeCloths.Insert(0, newCloth);

            // Map the new cloth to its corresponding spot.
            _clothSpotDict[newCloth] = _spots[i];

            // Increment the cloth count.
            _clothCount++;

            // Animate the new cloth to its correct spot.
            UniTask alignToStart = newCloth.transform
                .DOMove(_spots[i].position + Settings.BandAnimationData.spotOffset, Settings.BandAnimationData.shiftingDuration)
                .SetEase(Settings.BandAnimationData.shiftingEase)
                .OnComplete(() => newCloth.AdjustmentShakeAnim(isFirst))
                .WithCancellation(UniTaskCancellationExtensions.GetCancellationTokenOnDestroy(newCloth));

            // Add the animation task to the list of spawn tasks.
            spawnTasks.Add(alignToStart);
        }

        // Return the list of spawn tasks.
        return spawnTasks;
    }
    #endregion
}