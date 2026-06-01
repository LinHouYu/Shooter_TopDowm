using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How fast the bullet travels.")]
    private float speed = 20f;

    [SerializeField]
    [Tooltip("How long the bullet lives before destroying itself.")]
    private float lifeTime = 3f;

    private void Start()
    {
        // 子弹生成后，开始倒计时，时间到了就自动销毁（防止占内存）
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 每一帧让子弹沿着自己的正前方（Z轴）移动
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    
    // 如果子弹碰到带有 Collider 的物体，可以在这里处理伤害和销毁
    private void OnTriggerEnter(Collider other)
    {
        // 例如：如果碰到的不是玩家，就销毁子弹
        if (!other.CompareTag("Player"))
        {
            // TODO: 这里可以加上敌人掉血的代码
            Destroy(gameObject);
        }
    }
}