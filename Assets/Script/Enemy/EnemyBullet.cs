using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;
    
    [Tooltip("子弹造成的伤害")]
    public int damage = 10; // 新增：伤害值

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
        // 只要碰到的不是敌人自己，子弹就销毁
        if (!other.CompareTag("Enemy"))
        {
            // 如果碰到的是玩家，获取玩家脚本并扣血
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                    Debug.Log($"玩家被击中了！受到 {damage} 点伤害");
                }
            }
            
            // 造成伤害后销毁子弹
            Destroy(gameObject);
        }
    }
}