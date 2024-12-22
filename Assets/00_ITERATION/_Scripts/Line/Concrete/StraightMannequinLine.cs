using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class StraightMannequinLine : StraightLine<Mannequin>
{
    [Title("Initialize Settings")]

    [Title("Existing Content"), Space]

    [SerializeField]
    private Mannequin[] _mannequins;


    [SerializeField]
    Transform[] _linePoints;

    private void Start()
    {
        Initialize().Forget();
    }
    public override async UniTask Initialize(Mannequin[] values = null)
    {
        foreach (var info in LevelManager.CurrentLevel.MannequinLineInfos)
        {
            if (info.Line == this)
            {
                _mannequins = new Mannequin[info.Content.Count];

                for (int i = 0; i < info.Content.Count; i++)
                {
                    _mannequins[i] = Instantiate(info.Content[i], transform);
                    await UniTask.DelayFrame(1);
                }
                break;
            }
        }
        if (_mannequins == null || _mannequins.Length == 0)
        {
            Debug.LogWarning("Mannequin Line is empty");
            return;
        }
        // _mannequins = Content.ToArray();
        LineManager.Instance.RandomizeArray(ref _mannequins);
        await base.Initialize(_mannequins);

        GameManager.CurrentState.Value = GameState.Playing;
    }
    public override void ClearLine()
    {
        Content = new();
        /* 
        foreach (var mannequin in _mannequins)
            mannequin.transform.SetParent(null); */
        base.ClearLine();
    }
    public async override UniTask OrderQueue()
    {
        await base.OrderQueue();

    }
    public override Transform[] LinePoints { get => _linePoints; set => _linePoints = value; }
}
