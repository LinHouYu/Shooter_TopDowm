using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;

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
            // 如果碰到的是玩家，可以在这里添加扣血逻辑
            if (other.CompareTag("Player"))
            {
                Debug.Log("玩家被击中了！");
                // 扣血代码写这里
            }
            
            Destroy(gameObject);
        }
    }
}