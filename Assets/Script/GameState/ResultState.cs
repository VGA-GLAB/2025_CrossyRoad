using UnityEngine;

public class ResultState : IGameState
{
    public void Enter()
    {
        Debug.Log("リザルト");

        GameManager.instance.resultUI.SetActive(true);
        GameManager.instance.uIEffect.ButtonAppearanceAnimation();
    }

    public void Exit()
    {
        Debug.Log("リザルト終了");

        GameManager.instance.resultUI.SetActive(false);
    }

    public void Key() { }
}
