using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

public class DepositSpool : BaseSpool, IFillable
{
    [SerializeField] private bool _isFilled;
    [SerializeField] private Yarn myYarn;
    public YarnData filledYarnData;
    public Material depositYarnMaterial;
    private void Awake()
    {
        myYarn = GetComponentInChildren<Yarn>();
        depositYarnMaterial = myYarn.GetComponent<Renderer>().sharedMaterial;
        _contents = new() { myYarn };
    }
    [Button]
    public async UniTask Fill(YarnData data)
    {
        _inProgress = true;
        _isFilled = true;
        filledYarnData = data;
        myYarn.Tube.clipTo = 0;
        // myYarn.Tube.color = data.color;
        // depositYarnMaterial.color = data.color;
        myYarn.GetComponent<Renderer>().sharedMaterial = data.yarnMaterial;

        SoundManager.Instance.PlaySFX(SFXType.DepositFill);
        await YarnController.Instance.Rolling(myYarn, RollType.Roll, this, FillDuration);
        SoundManager.Instance.ResetPitch();
        _inProgress = false;
        transform.rotation = Quaternion.identity;
        myYarn.Spline.RebuildImmediate();
    }
    void PlayFillSound()
    {
        SoundManager.Instance.PlaySFX(SFXType.DepositFill, 0.1f);
    }
    public bool CanBeFilled(YarnData data)
    {
        return !_isFilled;
    }
    protected override void RemoveContent(int index)
    {
        myYarn.Tube.clipTo = 0f;
        _isFilled = false;
    }
    /// <summary>
    /// This method is responsible for bursting the yarn content in the spool.
    /// </summary>
    public async UniTask BurstContentAsync()
    {
        float defaultOffset = myYarn.Tube.offset.x;

        // Enlargen the yarn tube till it bursts.
        await DOTween.To(
            () => myYarn.Tube.offset.x,
            x => myYarn.Tube.offset = new Vector3(x, 0, 0),
            -0.15f,
            .2f
        ).ToUniTask();

        // Play the burst sound
        SoundManager.Instance.PlaySFX(SFXType.DepositPop);

        // Remove the yarn content
        RemoveContent(0);

        // Reset the yarn tube's offset to its default position
        myYarn.Tube.offset = new Vector3(defaultOffset, 0, 0);
    }

    public bool IsFilled { get => _isFilled; }
    public IConnect Connector => myYarn;
    public float FillDuration => Settings.KnittingSettings.RollingDuration;
}
