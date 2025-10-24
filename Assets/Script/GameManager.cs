using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UIEffect参照")]
    [SerializeField] private UIEffect uIEffect;
    public UIEffect UIEffect => uIEffect;

    [Header("InGameUIManager")]
    [SerializeField] private InGameUIManager gameUIManager;
    public InGameUIManager inGameUIManager => gameUIManager;

    [Header("ゲーム中")]
    [SerializeField] private bool isInGamePlay;
    public bool IsInGamePlay => isInGamePlay;

    [Header("プレイヤーが死んだとのイベント用")]
    [SerializeField] private PlayerMove _playerMove;

    private Dictionary<GameState, IGameState> gameStateDictionary; //ステートマシン管理
    private IGameState currentGameState; //現在のステート

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

        //ステートマシンを初期化時に、インスタンスを作成する
        gameStateDictionary = new Dictionary<GameState, IGameState>
        {
            {GameState.Title, new TitleState() },
            {GameState.InGame, new InGameState() },
            {GameState.Result, new ResultState() },
        };

        //タイトルの状態にする
        ChangeGameState(GameState.Title);
        if(_playerMove == null)
        {
            _playerMove = FindAnyObjectByType<PlayerMove>();
        }
        _playerMove.OnPlayerDeathAction += PlayerDead;
    }

    private void OnDestroy()
    {
        if (_playerMove != null)
        {
            _playerMove.OnPlayerDeathAction -= PlayerDead;
        }
    }

    private void Update()
    {
        //タイトルの入力を検知
        currentGameState?.Key();

        //後で消す
        if (Input.GetKeyDown(KeyCode.W)) //死亡確認用
        {
            ChangeGameState(GameState.Result);
        }
    }

    public void ChangeGameState(GameState state) //現在のステートを変更する
    {
        //同じステートだったら、処理を辞める
        if (currentGameState == gameStateDictionary[state]) return;

        //現在のステートを終わらせて、変更する
        currentGameState?.Exit();
        currentGameState = gameStateDictionary[state];
        currentGameState.Enter();
    }

    public void ChangeInGamePlay(bool isFlag) //ゲーム中かの状態切替
    {
        isInGamePlay = isFlag;
    }

    private void PlayerDead() //プレイヤーが死んだときの処理
    {
        ChangeGameState(GameState.Result);
    }
}
