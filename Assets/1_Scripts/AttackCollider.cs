using System.Collections;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private GameObject fatherPlayer;
    [SerializeField] public ChickenLouncher attacker;
    [SerializeField] private int attackType;

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
 
        if ("Enemy".Equals(other.gameObject.tag))
        {
            switch (attackType)
            {
                case 0:
                    GameManager.Instance.chickenEnemyTakeDamage(other.GetComponent<ContenedorEnemigo1>().gameObject, 1);
                    break;
                case 1:
                    GameManager.Instance.chickenEnemyTakeDamage(other.GetComponent<ContenedorEnemigo1>().gameObject, 2);
                    break;
            }

            //this.gameObject.SetActive(false);


        }

    }

}
