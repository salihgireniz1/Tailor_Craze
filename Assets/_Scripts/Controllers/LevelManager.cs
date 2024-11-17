using R3;
using System;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] private LevelContainer _levelContainer;
    [SerializeField] private bool automated;
    [SerializeField] private int _level = 1;
    [SerializeField] private int _loopStartLevel = 12;
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
        }

        ).AddTo(this);
    }
    /// <summary>
    /// Retrieves the level data based on the current level index.
    /// </summary>
    /// <returns>The level data for the current level.</returns>
    public static LevelData GetLevelData()
    {
        // Calculate the total number of levels
        int levelCount = Instance._levelContainer.Levels.Length;

        // Calculate the current level index within the range of available levels
        int index = Instance.LevelIndex % levelCount;

        // Define the loop start level index
        int loopLevelIndex = Instance._loopStartLevel - 1;

        // If the index falls within the range below _loopStartLevel after wrapping around, shift it to _loopStartLevel
        if (index < loopLevelIndex && Instance.LevelIndex >= levelCount)
        {
            index = loopLevelIndex + (index % (levelCount - loopLevelIndex));
        }

        // Return the level data for the current level
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
