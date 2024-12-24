using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpoolPlane : MonoBehaviour, IQueueable<SpoolPlane>, ISelectable
{
    public Transform CurrentStandPoint
    {
        get => _currentStandPoint;
        set
        {
            _currentStandPoint = value;
            transform.SetParent(_currentStandPoint);
        }
    }

    public bool CanBeSelected
    {
        get => _canBeSelected;
        set
        {
            _canBeSelected = value;
            _outline.enabled = value;
            _animator.enabled = value;
            if (value)
            {
                transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
            }
        }
    }

    public BaseLine<SpoolPlane> CurrentLine
    {
        get => _currentLine;
        set => _currentLine = value;
    }
    public Spool[] Spools => _spools;

    public bool IsEmpty => _spools.All(s => s.IsEmpty);
    public int Fillness
    {
        get
        {
            if (IsEmpty) return 0;
            return _spools.Count(s => !s.IsEmpty);
        }
    }

    [SerializeField]
    BaseLine<SpoolPlane> _currentLine;

    [SerializeField]
    private Transform _currentStandPoint;

    [SerializeField]
    private Spool[] _spools;

    [SerializeField]
    private bool _canBeSelected;
    [SerializeField]
    private float[] spoolAngles;

    [SerializeField]
    private SpriteRenderer _outline;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private GameObject _disableObject;

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject() || !CanBeSelected) return;
        Select().Forget();
    }
    public async UniTask BringSpoolForward(Spool spool)
    {
        if (_spools.Contains(spool))
        {
            int index = 0;
            foreach (var item in _spools)
            {
                if (item == spool) break;
                index++;
            }
            await transform.DORotate(Vector3.up * spoolAngles[index], .2f).SetEase(Ease.InBack).ToUniTask();
        }
    }

    [Button]
    public void GeneratePlane(List<YarnType> types)
    {
        for (int i = 0; i < _spools.Length; i++)
        {
            _spools[i].AddContent(types[i], false);
        }
    }

    public async UniTask Move(Vector3 position, float duration = 0F)
    {
        ControlTubeActivition(false);
        await transform.DOMove(position, duration).ToUniTask();
        ControlTubeActivition(true);
    }

    public async UniTask AlignToPoint(Transform newPoint, float duration = 0F)
    {
        if (duration == 0f)
        {
            transform.position = newPoint.position;
        }
        else
        {
            await Move(newPoint.position, duration);
        }
        CurrentStandPoint = newPoint;
    }

    public async UniTask Select()
    {
        if (DeskManager.Instance.FirstAvailableSpot() == default) return;
        CanBeSelected = false;
        var dequeuedThis = CurrentLine.ReturnFirst();

        _currentLine.OrderQueue().Forget();
        DeskManager.Instance.FillSpot(dequeuedThis).Forget();
    }

    public void ControlTubeActivition(bool activated)
    {
        foreach (var item in _spools)
        {
            item.TubeActivition(activated);
        }
    }
}
