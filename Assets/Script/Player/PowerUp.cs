using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum Type { Shield, Heal, SuperBuff }
    public Type powerUpType;

    // 核心修改：OnTriggerEnter2D 改成了 OnTriggerEnter，Collider2D 改成了 Collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                switch (powerUpType)
                {
                    case Type.Shield: player.ActivateShield(6f); break;
                    case Type.Heal: player.Heal(20); break;
                    case Type.SuperBuff: player.ActivateSuperBuff(10f); break;
                }
                Destroy(gameObject);
            }
        }
    }
}