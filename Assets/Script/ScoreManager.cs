using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int MaxScore { get; private set; } = 0;
    public int CurrentScore { get; private set; } = 0;
    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private TextMeshProUGUI _maxScore;
    [SerializeField] private TextMeshProUGUI _currentScore;
    [SerializeField] private Button _resetButton;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        if(_playerMove == null)
        {
            _playerMove = FindAnyObjectByType<PlayerMove>();
        }
        _playerMove.OnScoreUpAction += AddScore;
        _maxScore.text = "MaxScore : " + MaxScore.ToString();
        _currentScore.text = "Score : " + CurrentScore.ToString();
        if (_resetButton != null)
            _resetButton.onClick.AddListener(ResetScore);
    }

    private void OnDestroy()
    {
        _playerMove.OnScoreUpAction -= AddScore;
    }   

    public void AddScore()
    {
        CurrentScore++;
        if (CurrentScore > MaxScore)
        {
            MaxScore = CurrentScore;
        }
        _maxScore.text = "MaxScore : " + MaxScore.ToString();
        _currentScore.text = "Score : " + CurrentScore.ToString();
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        Debug.Log("スコアリセット！");
    }
}
