using UnityEngine;

public interface IGameState
{
    void Enter();
    void Exit();

    void Key();
}

public enum GameState
{
    Title,
    InGame,
    Result,
}
