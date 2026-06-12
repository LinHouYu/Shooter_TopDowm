using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
 
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private PlayerUIManager uiManager;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 20f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;
    
    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmo = 12;       
    [SerializeField] private float reloadTime = 1.5f; 
    private int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer;

    // ======== 道具状态变量 ========
    private bool isInvincible = false;
    private bool isSuperBuffed = false;
    // ==============================
 
    private Camera mainCamera;
    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector3 lookTarget;
    private float verticalVelocity;

    private bool isDashing;
    private float dashTimeLeft;
    private float dashCooldownTimer;
    private Vector3 dashDirection;

    private float fireTimer;
 
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentAmmo = maxAmmo;

        if (uiManager != null)
        {
            uiManager.UpdateHealthUI(currentHealth, maxHealth);
            uiManager.UpdateDashUI(1f, 0f);
            uiManager.UpdateAmmoUI(currentAmmo, maxAmmo, false);
        }
    }
    
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && dashCooldownTimer <= 0f && !isDashing)
        {
            StartDash();
        }
    }

    public void OnFire(InputValue value)
    {
        if (value.isPressed && fireTimer <= 0f && !isReloading)
        {
            if (currentAmmo > 0) Shoot();
            else StartReload();
        }
    }

    public void OnReload(InputValue value)
    {
        if (value.isPressed && !isReloading && currentAmmo < maxAmmo)
        {
            StartReload();
        }
    }

    public void OnLook(InputValue value)
    {
        Vector2 mouseScreenPosition = value.Get<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            lookTarget = ray.GetPoint(enter);
        }
    }
 
    private void Update()
    {
        HandleTimers(); 
        ApplyGravity();
        MovePlayer();
        RotateTowardsMouse();
    }

    private void HandleTimers()
    {
        if (dashCooldownTimer > 0) 
        {
            dashCooldownTimer -= Time.deltaTime;
            if (uiManager != null) 
                uiManager.UpdateDashUI(1f - (dashCooldownTimer / dashCooldown), dashCooldownTimer);
        }
        else if (!isDashing && uiManager != null) 
        {
            uiManager.UpdateDashUI(1f, 0f);
        }

        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0) isDashing = false;
        }

        if (fireTimer > 0) fireTimer -= Time.deltaTime;

        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
            {
                isReloading = false;
                currentAmmo = maxAmmo;
                if (uiManager != null) uiManager.UpdateAmmoUI(currentAmmo, maxAmmo, false);
            }
        }
    }

    private void Shoot()
    {
        fireTimer = fireRate; 
        currentAmmo--; 
        if (uiManager != null) uiManager.UpdateAmmoUI(currentAmmo, maxAmmo, false); 

        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    private void StartReload()
    {
        isReloading = true;
        reloadTimer = reloadTime;
        if (uiManager != null) uiManager.UpdateAmmoUI(currentAmmo, maxAmmo, true);
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;

        if (moveInput.sqrMagnitude > 0.01f)
            dashDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        else
            dashDirection = transform.forward;
    }
 
    private void ApplyGravity()
    {
        if (characterController.isGrounded) verticalVelocity = -1f;
        else verticalVelocity += gravity * Time.deltaTime;
    }
 
    private void MovePlayer()
    {
        Vector3 movement = isDashing 
            ? dashDirection * dashSpeed 
            : new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;

        movement.y = verticalVelocity;
        characterController.Move(movement * Time.deltaTime);
    }
 
    private void RotateTowardsMouse()
    {
        Vector3 lookDirection = lookTarget - transform.position;
        lookDirection.y = 0f;
 
        if (lookDirection.sqrMagnitude <= 0.001f) return;
 
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        // 如果处于无敌状态（吃到了护盾道具），则免疫伤害
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        if (uiManager != null) uiManager.UpdateHealthUI(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player Died!");
        }
    }

    // ======== 新增：道具效果接收函数 ========
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth); // 防止血量超出上限
        if (uiManager != null) uiManager.UpdateHealthUI(currentHealth, maxHealth);
    }

    public void ActivateShield(float duration)
    {
        StartCoroutine(ShieldRoutine(duration));
    }

    private IEnumerator ShieldRoutine(float duration)
    {
        isInvincible = true;
        Debug.Log("开启无敌护盾！");
        // 你可以在这里激活护盾特效物体
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        Debug.Log("护盾结束！");
    }

    public void ActivateSuperBuff(float duration)
    {
        StartCoroutine(SuperBuffRoutine(duration));
    }

    private IEnumerator SuperBuffRoutine(float duration)
    {
        if (isSuperBuffed) yield break; // 防止重复吃Buff导致数值错乱

        isSuperBuffed = true;
        Debug.Log("开启火力强化！");
        
        // 记录原始数值
        float originalFireRate = fireRate;
        float originalMoveSpeed = moveSpeed;

        // 强化效果：移速变为 1.5 倍，射击间隔减半（射速翻倍）
        moveSpeed *= 1.5f;
        fireRate *= 0.5f;

        yield return new WaitForSeconds(duration);

        // 恢复原始数值
        moveSpeed = originalMoveSpeed;
        fireRate = originalFireRate;
        isSuperBuffed = false;
        Debug.Log("火力强化结束！");
    }
    // =========================================
}