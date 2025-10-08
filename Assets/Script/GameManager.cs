using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UIEffect�Q��")]
    public UIEffect uIEffect;

    [Header("�^�C�g��UI")]
    public GameObject titleUI;
    [Header("�C���Q�[��UI")]
    public GameObject inGameUI;
    [Header("���U���gUI")]
    public GameObject resultUI;

    [Header("�Q�[����")]
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
        //�^�C�g���̓��͂����m
        currentGameState?.Key();

        if (Input.GetKeyDown(KeyCode.W)) //���S�m�F�p
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
