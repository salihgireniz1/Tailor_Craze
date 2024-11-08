using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] private LevelContainer _levelContainer;
    [SerializeField] private bool automated;
    [SerializeField] private int _level = 1;
    public static ReactiveProperty<int> LevelProperty { get; private set; } = new ReactiveProperty<int>(1);
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        LevelProperty.Value = Level;
        GameManager.CurrentState
        .Where(state => state == GameState.Victory)
        .Subscribe(_ =>
        {
            Level++;
            Debug.Log(GameManager.CurrentState.Value);
        }

        ).AddTo(this);
    }
    public static LevelData GetLevelData()
    {
        int index = Instance.LevelIndex % Instance._levelContainer.Levels.Length;
        return Instance._levelContainer.Levels[index];
    }
    [Button]
    void ResetLevel(int lvl = 1)
    {
        bool tmp = automated;
        automated = true;
        Level = Math.Max(lvl, 1);
        automated = tmp;
    }
    public static int Level
    {
        get
        {
            if (Instance.automated)
            {
                int lvl = ES3.Load(Consts.LevelKey, 1);
                Instance._level = lvl;
                return lvl;
            }

            return Instance._level;
        }
        set
        {
            Instance._level = value;
            LevelProperty.Value = value;
            Debug.Log("Level Changed " + value);
            if (Instance.automated) ES3.Save(Consts.LevelKey, Instance._level);
        }
    }
    public void LevelBackDebugger()
    {
        Level--;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LevelForwardDebugger()
    {
        Level++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public int LevelIndex => Math.Max(0, Level - 1);
}
