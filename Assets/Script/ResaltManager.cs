using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResaltManager : MonoBehaviour
{
    public static ResaltManager instance;

    [Header("プレイヤーが死んだとのイベント用")]
    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private GameObject _scorePanel;
    [SerializeField] private TextMeshProUGUI _maxScoreText;
    [SerializeField] private TextMeshProUGUI _currentScoreText;
    [SerializeField] private Button _retryButton;

    [Header("インゲーム中のテキスト")]
    [SerializeField] private TextMeshProUGUI _inGameMaxScoreText;
    [SerializeField] private TextMeshProUGUI _inGameCurrentScore;

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
        _playerMove.OnPlayerDeathAction += OnPlayerDeath;
    }

    private void Start()
    {
        _gameOverPanel.gameObject.SetActive(false);
        _scorePanel.gameObject.SetActive(false);
        _gameOverText.gameObject.SetActive(false);
        _maxScoreText.gameObject.SetActive(false);
        _currentScoreText.gameObject.SetActive(false);
        if (_retryButton != null)
            _retryButton.onClick.AddListener(RetryGame);
        _retryButton.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _playerMove.OnPlayerDeathAction -= OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        _inGameCurrentScore.gameObject.SetActive(false);
        _inGameMaxScoreText.gameObject.SetActive(false);
        _gameOverPanel.gameObject.SetActive(true);
        _scorePanel.gameObject.SetActive(true);
        _maxScoreText.gameObject.SetActive(true);
        _currentScoreText.gameObject.SetActive(true);
        _retryButton.gameObject.SetActive(true);
        _gameOverText.text = "GAME OVER";
        int max = ScoreManager.instance.MaxScore;
        int current = ScoreManager.instance.CurrentScore;
        _maxScoreText.text = "最高　記録 :" + max;
        _currentScoreText.text = "今回 :" + current;
    }

    private void RetryGame()
    {
        // UIをリセット
        _gameOverPanel.SetActive(false);
        _scorePanel.SetActive(false);
        _retryButton.gameObject.SetActive(false);
        _currentScoreText.gameObject.SetActive(false);
        _maxScoreText.gameObject.SetActive(false);
        _inGameCurrentScore.gameObject.SetActive(true);
        _inGameMaxScoreText.gameObject.SetActive(true);
        ScoreManager.instance.ResetScore();
        GameManager.instance.ChangeGameState(GameState.InGame);
    }
}
