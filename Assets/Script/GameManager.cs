using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UIEffect参照")]
    public UIEffect uIEffect;
    [Header("InGameUIManager")]
    [SerializeField] private InGameUIManager gameUIManager;
    public InGameUIManager inGameUIManager => gameUIManager;

    /*
    [Header("タイトルUI")]
    public GameObject titleUI;
    [Header("インゲームUI")]
    public GameObject inGameUI;
    [Header("リザルトUI")]
    public GameObject resultUI;
    */

    [Header("ゲーム中")]
    [SerializeField] private bool isInGamePlay;
    public bool IsInGamePlay => isInGamePlay;

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

        //後で消す
        if (Input.GetKeyDown(KeyCode.W)) //死亡確認用
        {
            ChangeIGameState(new ResultState());
        }
    }

    public void ChangeIGameState(IGameState state) //現在のステートを変更する
    {
        //同じステートだったら、処理を辞める
        if (currentGameState == state) return;

        //現在のステートを終わらせて、変更する
        currentGameState?.Exit();
        currentGameState = state;
        currentGameState.Enter();
    }

    public void ChangeInGamePlay(bool isFlag) //ゲーム中かの状態切替
    {
        isInGamePlay = isFlag;
    }
}
