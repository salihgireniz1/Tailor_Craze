using R3;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using DG.Tweening;
using TailorCraze.Haptic;
using System;

public class ClothPart : MonoBehaviour, IFillable, IConnect
{
    public int _requiredYarnCount = 1;
    [SerializeField] private int _currentFillness = 0;
    [SerializeField] private Knit[] _knits;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private GameObject[] _bands;
    [SerializeField] private FactoryCloth _myCloth;
    [SerializeField] private YarnType _type;
    [SerializeField] private Knit[] _checkpoints;
    Dictionary<int, List<Knit>> _knitAparts = new();
    private Knit currentKnit;
    private Animator _parentAnimator;

    private void Start()
    {
        _parentAnimator = GetComponentInParent<Animator>();
        DivideKnits();
        currentKnit = _knits[0];
    }
    private void DivideKnits()
    {
        // _knitAparts = DivideKnitsIntoParts(_requiredYarnCount);
        _knitAparts = DivideKnitsByReferences(_checkpoints);
    }

    public void InitializePart(YarnType Type)
    {
        _knits = GetComponentsInChildren<Knit>(true);
        this.Type = Type;
        YarnData data = YarnController.Instance.GetYarnData(Type);
        foreach (var knit in _knits)
        {
            knit.KnitMaterial = data.knitMaterial;
            knit.InitializeKnit(data.color);
        }
        _renderer.sharedMaterial = data.mannequinMaterial;
        // _renderer.sharedMaterial.color = new Color32(data.color.r, data.color.g, data.color.b, (byte)(data.color.a * 0.95f));
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
        _parentAnimator?.SetBool("Knitting", true);

        var path = _knitAparts[_currentFillness];
        _currentFillness++;
        await TravelPath(path).AttachExternalCancellation(UniTaskCancellationExtensions.GetCancellationTokenOnDestroy(this));

        if (_bands.Length > 0)
        {
            int bandIndex = Mathf.Min(_bands.Length - 1, _currentFillness - 1);
            _bands[bandIndex].SetActive(false);
        }

        _parentAnimator?.SetBool("Knitting", false);
        _filling = false;

        await MyCloth.DeselectRotate();
    }
    public async UniTask TravelPath(List<Knit> path)
    {
        float knitDuration = Settings.KnittingSettings.KnittingDuration;

        for (int i = 0; i < path.Count; i++)
        {
            currentKnit = path[i];
            if (i % Settings.KnittingSettings.knitJumpAmount == 0)
            {
                SoundManager.Instance.PlaySFX(SFXType.Knitted);

                if (i % 3 == 0) HapticManager.HapticPlay(HapticType.VibratePop);

                await currentKnit.Activate(knitDuration);
            }
            else
            {
                currentKnit.Activate(knitDuration).Forget();
            }
        }
    }
    Dictionary<int, List<Knit>> DivideKnitsByReferences(Knit[] referenceKnits)
    {
        Dictionary<int, List<Knit>> knitParts = new Dictionary<int, List<Knit>>();
        int startIndex = 0;

        // Iterate through the reference knits to divide into parts
        for (int i = 0; i < referenceKnits.Length; i++)
        {
            // Find the index of the current reference knit
            int referenceIndex = Array.IndexOf(_knits, referenceKnits[i]);

            // Add the range of knits from the start index to the reference index
            if (referenceIndex > -1) // Ensure the reference knit exists
            {
                List<Knit> part = _knits.Skip(startIndex).Take(referenceIndex - startIndex).ToList();
                knitParts.Add(i, part);

                // Update the start index for the next segment
                startIndex = referenceIndex;
            }
            else
            {
                throw new ArgumentException($"Reference knit {referenceKnits[i]} not found in _knits.");
            }
        }

        // Add the final part, which includes everything after the last reference knit
        List<Knit> lastPart = _knits.Skip(startIndex).ToList();
        knitParts.Add(referenceKnits.Length, lastPart);

        return knitParts;
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
    public float FillDuration
    {
        get
        {
            // float knittingDuration = Settings.KnittingSettings.KnittingDuration;
            float knittingDuration = Settings.KnittingSettings.KnittingFrameAmount * Time.fixedDeltaTime;
            float knitCount = (float)_knitAparts[_currentFillness].Count / Settings.KnittingSettings.knitJumpAmount;
            return (knittingDuration /*+ 0.01f*/) * knitCount;
        }
    }
}
