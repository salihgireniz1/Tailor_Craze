using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ZigZagLine : BaseLine<Mannequin>
{
    [SerializeField]
    Transform[] _linePoints;

    [SerializeField]
    private Mannequin[] _mannequins;
    public override Queue<Mannequin> LineQueue { get; set; }
    public override Transform[] LinePoints { get => _linePoints; set => _linePoints = value; }
    private void Start()
    {
        Initialize().Forget();
    }
    public override async UniTask Initialize(Mannequin[] values = null)
    {
        _mannequins = new Mannequin[Content.Count];

        for (int i = 0; i < LevelManager.CurrentLevel.MannequinLineInfos.Count; i++)
        {
            var instance = Instantiate(Content[i], transform);
            _mannequins[i] = instance;

            await UniTask.DelayFrame(1);
        }

        if (_mannequins == null || _mannequins.Length == 0)
        {
            Debug.LogWarning("Mannequin Line is empty");
            return;
        }

        // LineManager.Instance.RandomizeArray(ref _mannequins);

        LineQueue = new();

        for (int i = 0; i < _mannequins.Length; i++)
        {
            AddToLine(_mannequins[i]);
            _mannequins[i].AlignToPoint(_linePoints[i].transform, 0F).Forget();
        }

        GameManager.CurrentState.Value = GameState.Playing;
    }
    public override void ClearLine()
    {
        if (LinePoints != null)
        {
            foreach (Transform point in LinePoints)
            {
                if (point && point.childCount > 0) DestroyImmediate(point.GetChild(0).gameObject);
            }
        }

        LineQueue?.Clear();
    }
}
