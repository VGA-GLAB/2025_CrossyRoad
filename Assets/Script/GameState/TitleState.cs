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
        //プレイヤーの入力があったら、インゲームへと移る
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            GameManager.instance.ChangeGameState(GameState.InGame);
        }
    }
}
