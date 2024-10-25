using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Spool : BaseSpool
{
    [SerializeField]
    private float _contentOffset = .15f;

    private void OnMouseDown()
    {
        SelectionController.Instance.SelectSpool(this);
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
    public void AddContent(YarnType type)
    {
        var requestedYarn = YarnController.Instance.GetYarn(type);
        if (requestedYarn != null)
        {
            _contents.Add(requestedYarn);
            requestedYarn.transform.SetParent(_yarnHolder);
            requestedYarn.transform.localPosition = GetPosition(requestedYarn);
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