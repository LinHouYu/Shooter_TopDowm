using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How fast the bullet travels.")]
    private float speed = 20f;

    [SerializeField]
    [Tooltip("How long the bullet lives before destroying itself.")]
    private float lifeTime = 3f;

    [SerializeField]
    [Tooltip("子弹造成的伤害")]
    private int damage = 10; // 新增：伤害值

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // 碰到的不是玩家自己，就准备销毁
        if (!other.CompareTag("Player"))
        {
            // 新增：如果碰到的是敌人，让敌人扣血
            if (other.CompareTag("Enemy"))
            {
                EnemyAI enemy = other.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
            
            // 造成伤害（或打到墙壁）后销毁子弹
            Destroy(gameObject);
        }
    }
}