using UnityEngine;
using TMPro;

/// <summary>
/// 使用 RectTransform 修改宽度（sizeDelta）来控制 UI
/// </summary>
public class PlayerUIManager : MonoBehaviour
{
    [Header("UI Rects")]
    public RectTransform healthFillRect;
    public RectTransform dashFillRect;
    public RectTransform ammoFillRect; 

    [Header("UI 满长度设置")]
    public float healthBarFullWidth = 750f;
    public float dashBarFullWidth = 750f;
    public float ammoBarFullWidth = 750f;

    [Header("UI 文本")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI dashText;
    public TextMeshProUGUI ammoText;

    // 更新血量 UI
    public void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthFillRect != null)
        {
            float ratio = (float)currentHealth / maxHealth;
            ratio = Mathf.Clamp01(ratio); 
            // 直接算出当前应该有的宽度
            healthFillRect.sizeDelta = new Vector2(healthBarFullWidth * ratio, healthFillRect.sizeDelta.y);
        }
        if (healthText != null) healthText.text = $"{currentHealth} / {maxHealth}";
    }

    // 更新冲刺 CD UI
    public void UpdateDashUI(float ratio, float cooldownLeft)
    {
        if (dashFillRect != null)
        {
            ratio = Mathf.Clamp01(ratio);
            dashFillRect.sizeDelta = new Vector2(dashBarFullWidth * ratio, dashFillRect.sizeDelta.y);
        }

        if (dashText != null)
        {
            if (cooldownLeft > 0)
                dashText.text = cooldownLeft.ToString("F1") + "s";
            else
                dashText.text = "READY";
        }
    }

    // 更新子弹 UI
    public void UpdateAmmoUI(int currentAmmo, int maxAmmo, bool isReloading = false)
    {
        if (ammoFillRect != null)
        {
            float ratio = (float)currentAmmo / maxAmmo;
            ratio = Mathf.Clamp01(ratio);
            ammoFillRect.sizeDelta = new Vector2(ammoBarFullWidth * ratio, ammoFillRect.sizeDelta.y);
        }

        if (ammoText != null)
        {
            if (isReloading)
                ammoText.text = "RELOADING...";
            else
                ammoText.text = $"{currentAmmo} / {maxAmmo}";
        }
    }
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // 把玩家模型、状态和血量整个传送到下一关
    }
}