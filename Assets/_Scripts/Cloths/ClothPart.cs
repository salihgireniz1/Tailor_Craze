using R3;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using DG.Tweening;

public class ClothPart : MonoBehaviour, IFillable, IConnect
{
    public int _requiredYarnCount = 1;
    [SerializeField] private int _currentFillness = 0;
    [SerializeField] private Knit[] _knits;
    [SerializeField] private YarnType _type;
    [ShowInInspector] Dictionary<int, List<Knit>> _knitAparts = new();
    private Knit currentKnit;
    [SerializeField] private FactoryCloth _myCloth;

    private void Start()
    {
        DivideKnits();
        currentKnit = _knits[0];
    }
    [Button]
    private void DivideKnits()
    {
        _knitAparts = DivideKnitsIntoParts(_requiredYarnCount);
    }
    [Button]
    public void InitializePart(YarnType type)
    {
        _knits = GetComponentsInChildren<Knit>(true);
        this.Type = type;
        var data = YarnController.Instance.GetYarnData(type);
        foreach (var knit in _knits)
        {
            knit.InitializeKnit(data.color);
        }

        if (TryGetComponent(out SpriteRenderer sr))
        {
            sr.color = new Color32(data.color.r, data.color.g, data.color.b, (byte)(data.color.a * 0.65f));
        }
        else if (TryGetComponent(out Renderer rend))
        {
            rend.sharedMaterial.color = new Color32(data.color.r, data.color.g, data.color.b, (byte)(data.color.a * 0.95f));
        }
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
            await UniTask.Yield();
        }
        MyCloth.SelectRotate().Forget();
        _filling = true;
        var path = _knitAparts[_currentFillness];
        _currentFillness++;
        await TravelPath(path).AttachExternalCancellation(UniTaskCancellationExtensions.GetCancellationTokenOnDestroy(this));
        _filling = false;
        await MyCloth.DeselectRotate();
        // if (MyCloth != null && MyCloth.IsCompleted())
        // {
        //     currentKnit = null;
        //     await ClothsController.Instance.CompleteCloth(MyCloth);
        // }
        // else
        // {
        // }
    }
    public async UniTask TravelPath(List<Knit> path)
    {
        float knitDuration = Settings.Instance.KnittingSettings.KnittingDuration;
        foreach (var knit in path)
        {
            // Activate the knit
            var oneKnit = knit.Activate(knitDuration);
            var vibrateCloth = UniTask.CompletedTask;
            if (!_myCloth.IsRotating)
            {
                vibrateCloth = _myCloth.transform
                                .DOPunchRotation(Random.onUnitSphere, knitDuration, 3)
                                .ToUniTask();
            }

            currentKnit = knit;
            await UniTask.WhenAll(oneKnit, vibrateCloth);
        }
        // await _myCloth.transform.DORotate(Vector3.zero, 0f).ToUniTask();
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
    public FactoryCloth MyCloth { get => _myCloth; set => _myCloth = value; }
    public IConnect Connector => this;
    public YarnType Type { get => _type; private set => _type = value; }
    public Vector3 Position { get => currentKnit?.transform.position ?? Vector3.zero; }
    public bool IsFilled => _currentFillness >= _requiredYarnCount;
    public int YarnCountToFill => _requiredYarnCount;
    public float FillDuration => Settings.Instance.KnittingSettings.KnittingDuration * _knitAparts[_currentFillness].Count /*+ 20 * Time.deltaTime*/;
}
