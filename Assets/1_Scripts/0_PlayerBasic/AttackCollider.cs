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
        attackType = attacker.currentChickenType;
    }

    private void OnTriggerEnter(Collider other)
    {
 
        if ("Enemy".Equals(other.gameObject.tag))
        {
            switch (attackType)
            {
                case 0:
                    GameManager.Instance.ChickenEnemyTakeDamage(other.GetComponent<ContenedorEnemigo1>().gameObject, 1, true);
                    break;
                case 1:
                    GameManager.Instance.ChickenEnemyTakeDamage(other.GetComponent<ContenedorEnemigo1>().gameObject, 2, true);
                    break;
            }

            //this.gameObject.SetActive(false);

        }

        if ("Interactive".Equals(other.gameObject.tag))
        {
            InteractiveObject interactiveObject = other.gameObject.GetComponent<InteractiveObject>();
            if (interactiveObject != null)
            {
                interactiveObject.ExecuteInteraction();
            }

        }

    }

}
