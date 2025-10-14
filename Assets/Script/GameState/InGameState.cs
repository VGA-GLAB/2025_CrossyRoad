using UnityEngine;

public class InGameState : IGameState
{
    public void Enter()
    {
        Debug.Log("�C���Q�[��");

        //�C���Q�[�����̏�Ԃɂ���
        GameManager.instance.inGameUIManager.InGameUI.SetActive(true);
        GameManager.instance.ChangeInGamePlay(true);
    }

    public void Exit()
    {
        Debug.Log("�C���Q�[���I��");

        //�C���Q�[���I���̏�Ԃɂ���
        GameManager.instance.inGameUIManager.InGameUI.SetActive(false);
        GameManager.instance.ChangeInGamePlay(false);
    }

    public void Key() { }
}
