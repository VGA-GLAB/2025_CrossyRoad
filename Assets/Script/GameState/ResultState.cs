using UnityEngine;

public class ResultState : IGameState
{
    public void Enter()
    {
        Debug.Log("���U���g");

        GameManager.instance.inGameUIManager.ResultUI.SetActive(true);
        GameManager.instance.UIEffect.ButtonAppearanceAnimation();
    }

    public void Exit()
    {
        Debug.Log("���U���g�I��");

        GameManager.instance.inGameUIManager.ResultUI?.SetActive(false);
    }

    public void Key() { }
}
