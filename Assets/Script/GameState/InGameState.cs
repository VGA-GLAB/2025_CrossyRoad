using UnityEngine;

public class InGameState : IGameState
{
    public void Enter()
    {
        Debug.Log("�C���Q�[��");

        GameManager.instance.inGameUI.SetActive(true);
        GameManager.instance.isInGamePlay = true;
    }

    public void Exit()
    {
        Debug.Log("�C���Q�[���I��");

        GameManager.instance.inGameUI.SetActive(false);
        GameManager.instance.isInGamePlay = false;
    }

    public void Key() { }
}
