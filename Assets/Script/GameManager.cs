using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UIEffect�Q��")]
    public UIEffect uIEffect;
    [Header("InGameUIManager")]
    [SerializeField] private InGameUIManager gameUIManager;
    public InGameUIManager inGameUIManager => gameUIManager;

    /*
    [Header("�^�C�g��UI")]
    public GameObject titleUI;
    [Header("�C���Q�[��UI")]
    public GameObject inGameUI;
    [Header("���U���gUI")]
    public GameObject resultUI;
    */

    [Header("�Q�[����")]
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
        //�^�C�g���̓��͂����m
        currentGameState?.Key();

        //��ŏ���
        if (Input.GetKeyDown(KeyCode.W)) //���S�m�F�p
        {
            ChangeIGameState(new ResultState());
        }
    }

    public void ChangeIGameState(IGameState state) //���݂̃X�e�[�g��ύX����
    {
        //�����X�e�[�g��������A���������߂�
        if (currentGameState == state) return;

        //���݂̃X�e�[�g���I��点�āA�ύX����
        currentGameState?.Exit();
        currentGameState = state;
        currentGameState.Enter();
    }

    public void ChangeInGamePlay(bool isFlag) //�Q�[�������̏�Ԑؑ�
    {
        isInGamePlay = isFlag;
    }
}
