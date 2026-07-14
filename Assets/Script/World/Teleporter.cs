using UnityEngine;
using UnityEngine.SceneManagement; // 必须引入场景管理命名空间

public class Teleporter : MonoBehaviour
{
    [Header("Scenes2")]
    public string nextSceneName;

    // 当有物体进入触发器时调用（如果是2D游戏，请改成 OnTriggerEnter2D 和 Collider2D）
    void OnTriggerEnter(Collider other)
    {
        // 检查碰到这个方块的是不是玩家（请确保你的玩家物体标签设为了 "Player"）
        if (other.CompareTag("Player"))
        {
            // 加载下一个场景
            SceneManager.LoadScene(nextSceneName);
        }
    }
}