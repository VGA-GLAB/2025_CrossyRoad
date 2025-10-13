using UnityEngine;

public class InGameState : IGameState
{
    public void Enter()
    {
        Debug.Log("インゲーム");

        //インゲーム中の状態にする
        GameManager.instance.inGameUIManager.InGameUI.SetActive(true);
        GameManager.instance.ChangeInGamePlay(true);
    }

    public void Exit()
    {
        Debug.Log("インゲーム終了");

        //インゲーム終了の状態にする
        GameManager.instance.inGameUIManager.InGameUI.SetActive(false);
        GameManager.instance.ChangeInGamePlay(false);
    }

    public void Key() { }
}
