using UnityEngine;
using TMPro;

public class ScoreUIBinder : MonoBehaviour
{
    [Header("拖入当前场景的分数和时间UI")]
    public TextMeshProUGUI localScoreText;
    public TextMeshProUGUI localTimerText;

    void Start()
    {
        // 检查是不是忘了拖入 UI
        if (localScoreText == null || localTimerText == null)
        {
            Debug.LogError("❌ UI 绑定失败：你在 Scene2 的 ScoreUIBinder 面板里漏拖了 Text 组件！");
            return;
        }

        // 检查是不是找不到 ScoreManager
        if (ScoreManager.Instance != null)
        {
            Debug.Log("✅ 成功找到 ScoreManager，正在接管 Scene2 的 UI 显示！");
            ScoreManager.Instance.RegisterUI(localScoreText, localTimerText);
        }
        else
        {
            Debug.LogError("❌ UI 绑定失败：找不到 ScoreManager.Instance！");
        }
    }
}