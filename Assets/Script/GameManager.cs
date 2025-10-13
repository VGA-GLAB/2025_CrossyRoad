using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UIEffect�Q��")]
    [SerializeField] private UIEffect uIEffect;
    public UIEffect UIEffect => uIEffect;

    [Header("InGameUIManager")]
    [SerializeField] private InGameUIManager gameUIManager;
    public InGameUIManager inGameUIManager => gameUIManager;

    [Header("�Q�[����")]
    [SerializeField] private bool isInGamePlay;
    public bool IsInGamePlay => isInGamePlay;

    private Dictionary<GameState, IGameState> gameStateDictionary; //�X�e�[�g�}�V���Ǘ�
    private IGameState currentGameState; //���݂̃X�e�[�g

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

        //�X�e�[�g�}�V�������������ɁA�C���X�^���X���쐬����
        gameStateDictionary = new Dictionary<GameState, IGameState>
        {
            {GameState.Title, new TitleState() },
            {GameState.InGame, new InGameState() },
            {GameState.Result, new ResultState() },
        };

        //�^�C�g���̏�Ԃɂ���
        ChangeGameState(GameState.Title);

    }

    private void Update()
    {
        //�^�C�g���̓��͂����m
        currentGameState?.Key();

        //��ŏ���
        if (Input.GetKeyDown(KeyCode.W)) //���S�m�F�p
        {
            ChangeGameState(GameState.Result);
        }
    }

    public void ChangeGameState(GameState state) //���݂̃X�e�[�g��ύX����
    {
        //�����X�e�[�g��������A���������߂�
        if (currentGameState == gameStateDictionary[state]) return;

        //���݂̃X�e�[�g���I��点�āA�ύX����
        currentGameState?.Exit();
        currentGameState = gameStateDictionary[state];
        currentGameState.Enter();
    }

    public void ChangeInGamePlay(bool isFlag) //�Q�[�������̏�Ԑؑ�
    {
        isInGamePlay = isFlag;
    }
}
