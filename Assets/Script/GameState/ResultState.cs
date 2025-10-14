using UnityEngine;

public class ResultState : IGameState
{
    public void Enter()
    {
        Debug.Log("リザルト");

        GameManager.instance.inGameUIManager.ResultUI.SetActive(true);
        GameManager.instance.UIEffect.ButtonAppearanceAnimation();
    }

    public void Exit()
    {
        Debug.Log("リザルト終了");

        GameManager.instance.inGameUIManager.ResultUI?.SetActive(false);
    }

    public void Key() { }
}
