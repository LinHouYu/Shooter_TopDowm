using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTeleporter : MonoBehaviour
{
    [Header("要传送的下一个场景名字")]
    public string nextSceneName = "Scenes2";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 直接加载下一关，不管 UI 和 时间，ScoreUIBinder 会在新场景自动接管
            SceneManager.LoadScene(nextSceneName);
        }
    }
}