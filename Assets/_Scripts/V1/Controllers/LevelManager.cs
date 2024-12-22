using R3;
using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] private bool automated;
    [SerializeField] private int _level = 1;
    [SerializeField] private int _loopStartLevel = 12;

    public static Level CurrentLevel { get; private set; }
    public static ReactiveProperty<int> LevelProperty { get; private set; } = new ReactiveProperty<int>(1);
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        LevelProperty.Value = Level;

        GameManager.CurrentState
        .Subscribe(state =>
        {
            switch (state)
            {
                case GameState.Victory:
                    Level++;
                    break;
                case GameState.Initializing:
                    LoadLevel().AsUniTask().Forget();
                    break;
                default:
                    break;
            }
        }

        ).AddTo(this);
    }
    public int LevelSceneCount = 20;
    public Level[] Levels;
    public string ActiveSceneName = "Level 1";

    public async Task LoadLevel()
    {
        // Get the current active scene
        Scene activeScene = SceneManager.GetSceneByName(ActiveSceneName);

        // Determine the new level name
        string newLevelName = "Level " + (GetLoadableIndex() + 1);

        // Unload the current active scene asynchronously
        if (activeScene.isLoaded)
        {
            await SceneManager.UnloadSceneAsync(activeScene.name);
        }

        // Load the new scene asynchronously
        await SceneManager.LoadSceneAsync(newLevelName, LoadSceneMode.Additive);

        // Optionally, set the new scene as the active scene
        // SceneManager.SetActiveScene(SceneManager.GetSceneByName(newLevelName));
        ActiveSceneName = newLevelName;

        CurrentLevel = FindAnyObjectByType<Level>();
    }

    private int GetLoadableIndex()
    {
        // int levelCount = Instance.Levels.Length;
        int levelCount = LevelSceneCount;

        // Calculate the current level index within the range of available levels
        int index = Instance.LevelIndex % levelCount;

        // Define the loop start level index
        int loopLevelIndex = Instance._loopStartLevel - 1;

        // If the index falls within the range below _loopStartLevel after wrapping around, shift it to _loopStartLevel
        if (index < loopLevelIndex && Instance.LevelIndex >= levelCount)
        {
            index = loopLevelIndex + (index % (levelCount - loopLevelIndex));
        }
        return index;
    }
    /// <summary>
    /// Retrieves the level data based on the current level index.
    /// </summary>
    /// <returns>The level data for the current level.</returns>
    public static async UniTaskVoid EnableLevelPrefab()
    {
        await UniTask.Delay(100);
        // Calculate the total number of levels
        int index = Instance.GetLoadableIndex();

        // CurrentLevel = Instance.Levels[index];
        // Return the level data for the current level
        Instance.Levels[index].gameObject.SetActive(true);

        GameManager.CurrentState.Value = GameState.Playing;
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
