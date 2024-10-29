using R3;
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
        CurrentState.Value = GameState.Initializing;
    }

}