using UnityEngine;

public class ResultState : IGameState
{
    public void Enter()
    {
        Debug.Log("���U���g");

        GameManager.instance.inGameUIManager.ResultUI.SetActive(true);
        GameManager.instance.UIEffect.ButtonAppearanceAnimation();

        //SoundManager.instance.PlayBGM("リザルト");
        //CuePlay.instance.PlayBGM("BGM_Result");
    }

    public void Exit()
    {
        Debug.Log("���U���g�I��");

        GameManager.instance.inGameUIManager.ResultUI?.SetActive(false);

        //SoundManager.instance.StopBGM();

        //これがあるとプレイヤーが動かないバグが発生。
        //CuePlay.instance.StopBGM();
        //CuePlay.instance.PlayBGM("BGM_Title");
    }

    public void Key() { }
}
