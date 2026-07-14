using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI显示")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText; 

    public int score = 0; 
    private float elapsedTime = 0f; 
    
    public int savedPlayerHealth = 100;

    [Header("状态控制")]
    public bool isTimerRunning = true; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); 
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject); 
        }
    }

    void Start()
    {
        UpdateScoreUI();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    // ==========================================
    // 新增：每次进入新场景时，用来重新绑定新 UI
    // ==========================================
    public void RegisterUI(TextMeshProUGUI newScoreText, TextMeshProUGUI newTimerText)
    {
        scoreText = newScoreText;
        timerText = newTimerText;
        
        // 绑定后立刻更新一次显示
        UpdateScoreUI();
        UpdateTimerUI();
    }

    // ==========================================
    // 新增：重新开始游戏时，重置分数和时间
    // ==========================================
    public void ResetData()
    {
        score = 0;
        elapsedTime = 0f;
        isTimerRunning = true;
    }

    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + score;
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "TIME: " + GetFormattedTime();
        }
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 1000f) % 1000f);

        return string.Format("{0:D2}:{1:D2}:{2:D3}", minutes, seconds, milliseconds);
    }
}