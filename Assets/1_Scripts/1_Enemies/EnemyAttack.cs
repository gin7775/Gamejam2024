using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Collider attackCollider; 
    public int damageAmount = 10; 

    private void Start()
    {
        attackCollider.enabled = false; 
    }
    
    public void ActivateCollider()
    {
        attackCollider.enabled = true;
    }

    public void DeactivateCollider()
    {
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.ReceiveDamage(damageAmount);
            }
        }
    }

}
