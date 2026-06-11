using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI显示")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText; // 专门显示时间的TextMeshPro文本槽位

    private int score = 0;
    private float elapsedTime = 0f; // 记录过去的时间（秒）

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateScoreUI();
    }

    void Update()
    {
        // 累加每帧消耗的时间
        elapsedTime += Time.deltaTime;
        UpdateTimerUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
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
            // 计算分、秒、毫秒
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 1000f) % 1000f);

            // D2 代表保持2位数（如01），D3 代表保持3位数（如005）
            timerText.text = string.Format("TIME: {0:D2}:{1:D2}:{2:D3}", minutes, seconds, milliseconds);
        }
    }
    
    
}