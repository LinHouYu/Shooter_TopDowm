using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement & Distances")]
    public float detectionRadius = 15f;
    public float idealDistance = 8f;
    public float moveSpeed = 3f;
    public float gravity = -9.81f;

    [Header("Dodge Settings")]
    public float bulletDetectionRadius = 3f;
    public float dodgeSpeed = 10f;
    public float dodgeDuration = 0.3f;
    public float dodgeCooldown = 1.5f;

    [Header("Random Walk Settings")]
    public float wanderRadius = 5f;
    public float wanderTimer = 2f;

    // ======== 新增：射击设置 ========
    [Header("Shooting Settings")]
    [Tooltip("敌人的子弹预制体")]
    public GameObject bulletPrefab;
    [Tooltip("子弹发射点（枪口）")]
    public Transform firePoint;
    [Tooltip("射击间隔时间（秒）")]
    public float fireRate = 1.5f;
    // =================================

    private Transform player;
    private CharacterController characterController;
    private float verticalVelocity;

    private bool isDodging;
    private float dodgeTimeLeft;
    private float dodgeCooldownTimer;
    private Vector3 dodgeDirection;

    private Vector3 wanderTarget;
    private float timer;
    
    // 射击冷却计时器
    private float fireTimer;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        SetNewWanderTarget();
    }

    private void Update()
    {
        ApplyGravity();
        HandleTimers();

        if (isDodging)
        {
            PerformDodge();
            return;
        }

        if (CheckForIncomingBullets()) return;

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRadius)
            {
                CombatBehavior(distanceToPlayer);
            }
            else
            {
                WanderBehavior();
            }
        }
    }

    private void HandleTimers()
    {
        if (dodgeCooldownTimer > 0) dodgeCooldownTimer -= Time.deltaTime;
        if (timer > 0) timer -= Time.deltaTime;
        else SetNewWanderTarget();

        // ======== 新增：更新射击冷却 ========
        if (fireTimer > 0) fireTimer -= Time.deltaTime;
        // =====================================
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded) verticalVelocity = -1f;
        else verticalVelocity += gravity * Time.deltaTime;
    }

    private void CombatBehavior(float distance)
    {
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0f;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), 10f * Time.deltaTime);
        }

        Vector3 movement = Vector3.zero;

        if (distance > idealDistance + 1f) movement = transform.forward * moveSpeed;
        else if (distance < idealDistance - 1f) movement = -transform.forward * moveSpeed;
        else movement = (transform.right * (Mathf.Sin(Time.time) * moveSpeed * 0.5f));

        movement.y = verticalVelocity;
        characterController.Move(movement * Time.deltaTime);

        // ======== 新增：开火逻辑 ========
        // 如果玩家在视野内，且射击冷却完毕，则开火
        if (fireTimer <= 0f)
        {
            ShootAtPlayer();
        }
        // =================================
    }

    // ======== 新增：执行射击 ========
    private void ShootAtPlayer()
    {
        fireTimer = fireRate; // 重置射击冷却

        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
    // =================================

    private void WanderBehavior()
    {
        Vector3 direction = wanderTarget - transform.position;
        direction.y = 0f;

        if (direction.magnitude > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);
            Vector3 movement = transform.forward * moveSpeed;
            movement.y = verticalVelocity;
            characterController.Move(movement * Time.deltaTime);
        }
    }

    private void SetNewWanderTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        wanderTarget = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        timer = wanderTimer;
    }

    private bool CheckForIncomingBullets()
    {
        if (dodgeCooldownTimer > 0) return false;

        Collider[] hits = Physics.OverlapSphere(transform.position, bulletDetectionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("PlayerBullet")) 
            {
                StartDodge(hit.transform.position);
                return true;
            }
        }
        return false;
    }

    private void StartDodge(Vector3 bulletPosition)
    {
        isDodging = true;
        dodgeTimeLeft = dodgeDuration;
        dodgeCooldownTimer = dodgeCooldown;

        Vector3 bulletToEnemy = transform.position - bulletPosition;
        bulletToEnemy.y = 0f;

        float dodgeSide = Random.value > 0.5f ? 1f : -1f;
        dodgeDirection = Vector3.Cross(bulletToEnemy.normalized, Vector3.up) * dodgeSide;
    }

    private void PerformDodge()
    {
        dodgeTimeLeft -= Time.deltaTime;
        Vector3 movement = dodgeDirection * dodgeSpeed;
        movement.y = verticalVelocity;
        characterController.Move(movement * Time.deltaTime);
        if (dodgeTimeLeft <= 0) isDodging = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, idealDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, bulletDetectionRadius);
    }
}