using UnityEngine;
using UnityEngine.InputSystem;
 
/// <summary>
/// Controls player movement, rotation, dashing, shooting, and communicates with the UIManager.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("拖入挂载了 PlayerUIManager 的物体")]
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
    
    // ======== 新增：子弹与换弹设置 ========
    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmo = 12;       // 最大弹匣12发
    [SerializeField] private float reloadTime = 1.5f; // 换弹所需时间
    private int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer;
    // =================================
 
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
        // 初始化数据并刷新一次UI
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
        // 只有按下按键，射击冷却结束，且不在换弹时才能射击
        if (value.isPressed && fireTimer <= 0f && !isReloading)
        {
            if (currentAmmo > 0)
            {
                Shoot();
            }
            else
            {
                // 如果没子弹了还尝试开火，自动触发换弹
                StartReload();
            }
        }
    }

    // ======== 新增：手动换弹输入 ========
    // 前提：在 Input System 中设置一个 Action 叫 Reload，绑定R键
    public void OnReload(InputValue value)
    {
        if (value.isPressed && !isReloading && currentAmmo < maxAmmo)
        {
            StartReload();
        }
    }
    // =================================

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
        else if (!isDashing && uiManager != null) // CD结束
        {
            uiManager.UpdateDashUI(1f, 0f);
        }

        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0) isDashing = false;
        }

        // 射击冷却
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
        currentAmmo--; // 消耗子弹
        if (uiManager != null) uiManager.UpdateAmmoUI(currentAmmo, maxAmmo, false); // 更新UI

        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogWarning("Bullet Prefab or Fire Point is not assigned in the Inspector!");
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
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        if (uiManager != null) uiManager.UpdateHealthUI(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            // 死亡逻辑可以在这里添加
            Debug.Log("Player Died!");
        }
    }
}