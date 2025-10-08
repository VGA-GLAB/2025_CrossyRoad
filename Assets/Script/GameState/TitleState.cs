using UnityEngine;

public class TitleState : IGameState
{
    public void Enter()
    {
        Debug.Log("タイトル");

        GameManager.instance.titleUI.SetActive(true);
        GameManager.instance.uIEffect.TitleFadeIn();
    }

    public void Exit()
    {
        Debug.Log("タイトル終了");

        GameManager.instance.uIEffect.TitleFadeOut();
    }

    public void Key()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.instance.ChangeIGameState(new InGameState());
        }
    }
}
