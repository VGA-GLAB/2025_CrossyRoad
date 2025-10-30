using System.IO;
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
    private string _saveFilePath;

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
        //保存パスを決定
        _saveFilePath = Path.Combine(Application.persistentDataPath, "scoreData.json");
        LoadMaxScore();
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
            SaveMaxScore();
        }
        _maxScore.text = "MaxScore : " + MaxScore.ToString();
        _currentScore.text = "Score : " + CurrentScore.ToString();
    }

    /// <summary>
    /// 最高スコアを保存する
    /// </summary>
    private void SaveMaxScore()
    {
        ScoreData data = new ScoreData { maxScore = MaxScore };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(_saveFilePath, json);
    }

    private void LoadMaxScore()
    {
        if (File.Exists(_saveFilePath))
        {
            string json = File.ReadAllText(_saveFilePath);
            ScoreData data = JsonUtility.FromJson<ScoreData>(json);
            MaxScore = data.maxScore;
        }
        else
        {
            Debug.Log("保存ファイルなし");
            MaxScore = 0;
        }
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        Debug.Log("スコアリセット！");
    }

    [System.Serializable]
    private class ScoreData
    {
        public int maxScore;
    }
}
