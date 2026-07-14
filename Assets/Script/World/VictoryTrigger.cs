using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; // 必须引入，用于加载场景

public class VictoryTrigger : MonoBehaviour
{
    [Header("胜利菜单 UI 设置")]
    [Tooltip("把你做好的胜利界面的父级 GameObject (Panel) 拖到这里")]
    public GameObject victoryMenuUI; 
    
    [Tooltip("胜利界面上用来显示最终分数的 TextMeshPro")]
    public TextMeshProUGUI finalScoreText; 
    
    [Tooltip("胜利界面上用来显示最终时间的 TextMeshPro")]
    public TextMeshProUGUI finalTimeText;  

    void Start()
    {
        // 游戏开始时，确保胜利菜单是隐藏的
        if (victoryMenuUI != null)
        {
            victoryMenuUI.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerVictoryMenu();
        }
    }

    void TriggerVictoryMenu()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.isTimerRunning = false;

            if (finalScoreText != null)
            {
                finalScoreText.text = "FINAL SCORE: " + ScoreManager.Instance.score;
            }
            
            if (finalTimeText != null)
            {
                finalTimeText.text = "FINAL TIME: " + ScoreManager.Instance.GetFormattedTime();
            }
        }

        if (victoryMenuUI != null)
        {
            victoryMenuUI.SetActive(true);
        }

        Time.timeScale = 0f; 

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ==========================================
    // 新增：给 UI 按钮调用的方法
    // ==========================================
    public void GoToMainScene()
    {
        // 1. 恢复游戏时间（非常重要！）
        Time.timeScale = 1f; 
        
        // 2. 加载名为 "Main" 的场景
        SceneManager.LoadScene("Main");
    }
}