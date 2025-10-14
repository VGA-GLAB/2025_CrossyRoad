using UnityEngine;

public class TitleState : IGameState
{
    public void Enter()
    {
        Debug.Log("�^�C�g��");

        GameManager.instance.inGameUIManager.TitleUI.SetActive(true);
        GameManager.instance.UIEffect.TitleFadeIn();
    }

    public void Exit()
    {
        Debug.Log("�^�C�g���I��");

        GameManager.instance.UIEffect.TitleFadeOut();
    }

    public void Key()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.instance.ChangeGameState(GameState.InGame);
        }
    }
}
