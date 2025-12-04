using UnityEngine;

public class ResultState : IGameState
{
    public void Enter()
    {
        Debug.Log("���U���g");

        GameManager.instance.inGameUIManager.ResultUI.SetActive(true);
        GameManager.instance.UIEffect.ButtonAppearanceAnimation();
        
        SoundManager.instance.PlayBGM("リザルト");
    }

    public void Exit()
    {
        Debug.Log("���U���g�I��");

        GameManager.instance.inGameUIManager.ResultUI?.SetActive(false);
        
        SoundManager.instance.StopBGM();
    }

    public void Key() { }
}
