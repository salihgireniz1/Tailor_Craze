using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class Spool : BaseSpool
{
    [SerializeField]
    private float _contentOffset = .15f;

    private void OnMouseDown()
    {
        if (GameManager.CurrentState.Value != GameState.Playing) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        SelectionController.Instance.SelectSpool(this);
        foreach (Yarn yarn in _contents)
        {
            if (yarn is HiddenYarn)
            {
                ((HiddenYarn)yarn).Reveal().Forget();
            }
        }
    }
    [Button]
    public void ClearContents()
    {
        foreach (var yarn in _contents)
        {
            if (yarn)
                DestroyImmediate(yarn.gameObject);
        }
        _contents.Clear();
    }
    [Button]
    public void AddContent(YarnType type, bool isHidden)
    {
        YarnData data = YarnController.Instance.GetYarnData(type);
        Yarn requestedYarn = YarnController.Instance.GetYarn(isHidden);
        requestedYarn.InitializeYarn(data);

        if (requestedYarn != null)
        {
            _contents.Add(requestedYarn);
            requestedYarn.transform.SetParent(_yarnHolder);
            requestedYarn.transform.localPosition = GetPosition(requestedYarn);
            requestedYarn.transform.localScale = Vector3.one;
            requestedYarn.Spline.RebuildImmediate();
        }
    }

    protected override void RemoveContent(int index)
    {
        if (index < 0 || index >= _contents.Count)
        {
            Debug.LogError("Please enter a valid index.");
            return;
        }
        var content = _contents[index];
        // RemoveContent(_contents[index]);

        try
        {
            _contents.Remove(content);
            DestroyImmediate(content.gameObject);
            foreach (var yarn in _contents)
            {
                yarn.transform.localPosition = GetPosition(yarn);
            }
        }
        catch (Exception)
        {
            Debug.LogError("Content does not contain selected yarn.");
        }
    }
    public void RemoveContent(Yarn content)
    {
        try
        {
            _contents.Remove(content);
            DestroyImmediate(content.gameObject);
            foreach (var yarn in _contents)
            {
                yarn.transform.localPosition = GetPosition(yarn);
            }
        }
        catch (Exception)
        {
            Debug.LogError("Content does not contain selected yarn.");
        }
    }
    private Vector3 GetPosition(Yarn content)
    {
        if (!_contents.Contains(content)) return default;
        int index = _contents.IndexOf(content);
        return new Vector3(0, _contentOffset * index * 3f, 0);
    }

}