using UnityEngine;

public class InGameState : IGameState
{
    public void Enter()
    {
        Debug.Log("インゲーム");

        GameManager.instance.inGameUI.SetActive(true);
        GameManager.instance.isInGamePlay = true;
    }

    public void Exit()
    {
        Debug.Log("インゲーム終了");

        GameManager.instance.inGameUI.SetActive(false);
        GameManager.instance.isInGamePlay = false;
    }

    public void Key() { }
}
