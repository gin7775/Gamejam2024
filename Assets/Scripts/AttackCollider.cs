using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] GameObject fatherPlayer;
    ChickenLouncher attacker;
    [SerializeField] int attackType;

    private void Start()
    {
        LounchAttack();
    }
    void LounchAttack()
    { 
        attacker = fatherPlayer.GetComponent<ChickenLouncher>();
        attackType = attacker.chickenType;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            switch (attackType)
            {
                case 0:
                    Debug.Log("Enemy hitted with headbut");
                    
                    attacker.DealDamage(other.gameObject, 1);

                    break;
                case 1:
                    Debug.Log("Enemy hitted with chicken");
                    attacker.DealDamage(other.gameObject, 2);
                    break;
            }
                    
            


        }
    }
}
