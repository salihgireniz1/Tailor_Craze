using R3;
using UnityEngine;
public enum GameState
{
    DEFAULT,
    Initializing,
    MainMenu,
    Playing,
    InProgress,
    GameOver,
    Victory
}

public class GameManager : MonoSingleton<GameManager>
{
    public static ReactiveProperty<GameState> CurrentState { get; set; } = new(GameState.DEFAULT);

    private void Start()
    {
        // Application.targetFrameRate = 60;
        CurrentState.Value = GameState.Initializing;
        // CurrentState.Subscribe(state => Debug.Log(state)).AddTo(this);
    }

}