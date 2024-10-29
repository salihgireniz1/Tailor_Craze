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

    private void Start()
    {
        GameManager.CurrentState
        .Where(state => state == GameState.Initializing)
        .Subscribe(
            _ => InitLevelCloths()
        ).AddTo(this);
    }
    public void CompleteCloth(FactoryCloth cloth)
    {
        if (_clothSpotDict.ContainsKey(cloth))
        {
            _clothSpotDict.Remove(cloth);
        }
        if (activeCloths.Contains(cloth))
        {
            activeCloths.Remove(cloth);
        }
        // Actually, earn some golds etc.
        Destroy(cloth.gameObject);

        if (_clothCount >= _levelCloths.Length && activeCloths.Count <= 0)
        {
            Debug.Log("WIN!");
            GameManager.CurrentState.Value = GameState.Victory;
        }
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
        AddNewClothAndShiftRight().Forget();
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
            int nextSpotIndex = _spots.ToList().IndexOf(currentSpot) + 1;
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
            UniTask alignToStart = newCloth.transform.DOMove(_spots[0].position + _offset, .5f).SetEase(Ease.OutBack).ToUniTask();
            shifts.Add(alignToStart);

            activeCloths.Insert(0, newCloth); // Add new cloth to the beginning of the list
            _clothSpotDict[newCloth] = _spots[0];
            _clothCount++;
        }

        await UniTask.WhenAll(shifts);
        // Await deposits to control new cloth.
        await DepositSpoolController.Instance.CheckNewClothAsync(newCloth);
        GameManager.CurrentState.Value = GameState.Playing;
    }
}
