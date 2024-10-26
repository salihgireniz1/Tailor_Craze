using R3;
using System;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class ClothPart : MonoBehaviour, IFillable, IConnect
{
    [SerializeField] private int _requiredYarnCount = 1;
    [SerializeField] private int _currentFillness = 0;
    [SerializeField] private Knit[] _knits;
    [SerializeField] private YarnType _type;
    Dictionary<int, List<Knit>> _knitAparts = new();
    private Knit currentKnit;

    private void Start()
    {
        DivideKnits();
        Position.Value = _knits[0].transform.position;
    }
    [Button]
    private void DivideKnits()
    {
        _knitAparts = DivideKnitsIntoParts(_requiredYarnCount);
    }
    [Button]
    public void InitializePart(YarnType type)
    {
        _knits = GetComponentsInChildren<Knit>();
        this.Type = type;
        var data = YarnController.Instance.GetYarnData(type);
        foreach (var knit in _knits)
        {
            knit.InitializeKnit(data.color);
        }
        GetComponent<SpriteRenderer>().color = new Color32(data.color.r, data.color.g, data.color.b, (byte)(data.color.a / 4));
    }
    public bool CanBeFilled(YarnData data)
    {
        return this.Type == data.Type && !IsFilled;
    }
    private bool _filling;
    public async UniTask Fill(YarnData data)
    {
        while (_filling)
        {
            Debug.Log("Waiting to fill...");
            await UniTask.Yield();
        }
        var path = _knitAparts[_currentFillness];
        _currentFillness++;
        await TravelPath(path);
        Debug.Log("Cloth Filled.");
    }
    public async UniTask TravelPath(List<Knit> path)
    {
        float knitDuration = ClothsController.Instance.knittingDuration;
        Debug.Log("Estimated duration to fill: " + (knitDuration * path.Count));
        float current = Time.time;
        _filling = true;
        foreach (var knit in path)
        {
            // Activate the knit
            var task = knit.Activate(knitDuration);
            currentKnit = knit;
            Position.Value = knit.transform.position;
            await task;
        }
        _filling = false;
        Debug.Log("Fill duration: " + (Time.time - current));
    }
    Dictionary<int, List<Knit>> DivideKnitsIntoParts(int n)
    {
        Dictionary<int, List<Knit>> knitParts = new Dictionary<int, List<Knit>>();

        // Calculate the size of each part
        int partSize = Mathf.CeilToInt((float)_knits.Length / n);

        for (int i = 0; i < n; i++)
        {
            // Get the subset of the knits for this part
            List<Knit> part = _knits.Skip(i * partSize).Take(partSize).ToList();

            // Add it to the dictionary
            knitParts.Add(i, part);
        }

        return knitParts;
    }
    public IConnect Connector => this;
    public YarnType Type { get => _type; private set => _type = value; }
    public ReactiveProperty<Vector3> Position { get; } = new();
    public bool IsFilled => _currentFillness >= _requiredYarnCount;
    public int YarnCountToFill => _requiredYarnCount;
    public float FillDuration => ClothsController.Instance.knittingDuration * (_knitAparts[_currentFillness].Count + 1);
}
