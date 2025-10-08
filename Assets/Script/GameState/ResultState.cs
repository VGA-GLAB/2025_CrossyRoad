using UnityEngine;

public class ResultState : IGameState
{
    public void Enter()
    {
        Debug.Log("���U���g");

        GameManager.instance.resultUI.SetActive(true);
        GameManager.instance.uIEffect.ButtonAppearanceAnimation();
    }

    public void Exit()
    {
        Debug.Log("���U���g�I��");

        GameManager.instance.resultUI.SetActive(false);
    }

    public void Key() { }
}
