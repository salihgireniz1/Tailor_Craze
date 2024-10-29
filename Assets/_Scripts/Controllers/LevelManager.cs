using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] private LevelContainer _levelContainer;
    [SerializeField] private bool automated;
    [SerializeField] private int _level = 1;
    private void Start()
    {
        GameManager.CurrentState
        .Where(state => state == GameState.Victory)
        .Subscribe(
            _ => Level++
        ).AddTo(this);
    }
    public static int Level
    {
        get
        {
            if (Instance.automated && ES3.KeyExists(Consts.LevelKey))
            {
                return ES3.Load(Consts.LevelKey, 1);
            }

            return Instance._level;
        }
        set
        {
            Instance._level = value;
            if (Instance.automated) ES3.Save(Consts.LevelKey, Instance._level);
        }
    }
    public static LevelData GetLevelData()
    {
        int index = Instance.LevelIndex % Instance._levelContainer.Levels.Length;
        return Instance._levelContainer.Levels[index];
    }
    public int LevelIndex => Math.Max(0, Level - 1);
}
