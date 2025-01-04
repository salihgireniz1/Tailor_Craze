using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpoolLine : StraightLine<SpoolPlane>
{
    [Title("Content Settings"), Space]

    public List<PlaneSpools> LineContent;

    [Title("Existing Content"), Space]
    [SerializeField] Transform[] _linePoints;

    [SerializeField] private SpoolPlane[] _planes;

    private void Start()
    {
        // LineQueue = new(_planes);
        Initialize(null).Forget();
    }

    public override async UniTask Initialize(SpoolPlane[] values)
    {
        foreach (var item in this._planes)
        {
            if (item) DestroyImmediate(item.gameObject);
        }

        while (LevelManager.CurrentLevel == null) await UniTask.Yield();
        
        foreach (var item in LevelManager.CurrentLevel.SpoolLineInfos)
        {
            if (item.Line == this)
            {
                LineContent = item.Content;
                break;
            }
        }
        var _planes = new SpoolPlane[LineContent.Count];
        for (int i = 0; i < LineContent.Count; i++)
        {
            var newPlane = Instantiate(Settings.GameData.PlanePrefab, transform);
            newPlane.GeneratePlane(LineContent[i].SpoolTypes);
            // newPlane.CurrentLine = this;
            _planes[i] = newPlane;
            await UniTask.DelayFrame(1);
        }
        await base.Initialize(_planes);

        (_planes[0] as ISelectable).CanBeSelected = true;

        this._planes = _planes;
    }
    [SerializeField] private Renderer _bandRenderer;
    public override async UniTask OrderQueue()
    {
        DOTween.To(
            () => _bandRenderer.material.mainTextureOffset,
            x => _bandRenderer.material.mainTextureOffset = x,
            _bandRenderer.material.mainTextureOffset + Vector2.up,
            0.5F
        );
        await base.OrderQueue();
        var firstMemberOfQueue = PeekFirst();
        if (firstMemberOfQueue != default)
        {
            firstMemberOfQueue.CanBeSelected = true;
        }
    }

    [Button(SdfIconType.AlignCenter)]
    void DebugDestroyFirst()
    {
        var firstSpoolTable = ReturnFirst();
        if (firstSpoolTable && firstSpoolTable != default) DestroyImmediate(firstSpoolTable.gameObject);
    }

    public override Transform[] LinePoints { get => _linePoints; set => _linePoints = value; }
}
