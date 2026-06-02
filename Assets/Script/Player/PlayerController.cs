using UnityEngine;
using UnityEngine.InputSystem;
 
/// <summary>
/// Controls player movement, rotation, dashing, and shooting for a 3D top-down shooter prototype.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 20f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    // ======== 新增：射击设置 ========
    [Header("Shooting Settings")]
    [SerializeField]
    [Tooltip("The bullet prefab to spawn.")]
    private GameObject bulletPrefab;

    [SerializeField]
    [Tooltip("The transform where the bullet will be spawned (e.g., the gun barrel).")]
    private Transform firePoint;

    [SerializeField]
    [Tooltip("Time between shots in seconds.")]
    private float fireRate = 0.2f;
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

    // 射击冷却计时器
    private float fireTimer;
 
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
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

    // ======== 新增：接收开火输入 ========
    public void OnFire(InputValue value)
    {
        // 如果按下按键，并且射击冷却已经结束
        if (value.isPressed && fireTimer <= 0f)
        {
            Shoot();
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
        HandleTimers(); // 更新所有冷却时间
        ApplyGravity();
        MovePlayer();
        RotateTowardsMouse();
    }

    private void HandleTimers()
    {
        // 冲刺冷却
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0) isDashing = false;
        }

        // 射击冷却
        if (fireTimer > 0) fireTimer -= Time.deltaTime;
    }

    // ======== 新增：射击逻辑 ========
    private void Shoot()
    {
        fireTimer = fireRate; // 重置冷却时间

        if (bulletPrefab != null && firePoint != null)
        {
            // 在 FirePoint 的位置，以 FirePoint 的旋转角度，生成子弹
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogWarning("Bullet Prefab or Fire Point is not assigned in the Inspector!");
        }
    }
    // =================================

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
}