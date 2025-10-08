using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UIEffect参照")]
    public UIEffect uIEffect;

    [Header("タイトルUI")]
    public GameObject titleUI;
    [Header("インゲームUI")]
    public GameObject inGameUI;
    [Header("リザルトUI")]
    public GameObject resultUI;

    [Header("ゲーム中")]
    public bool isInGamePlay;

    private IGameState currentGameState;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        ChangeIGameState(new TitleState());
    }

    private void Update()
    {
        //タイトルの入力を検知
        currentGameState?.Key();

        if (Input.GetKeyDown(KeyCode.W)) //死亡確認用
        {
            ChangeIGameState(new ResultState());
        }
    }

    public void ChangeIGameState(IGameState state)
    {
        if (currentGameState == state) return;

        currentGameState?.Exit();
        currentGameState = state;
        currentGameState.Enter();
    }
}
