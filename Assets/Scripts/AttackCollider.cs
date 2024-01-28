using System.Collections;
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
        if ("Enemy".Equals(other.gameObject.tag))
        {
            /*if (other.GetComponent<ContenedorEnemigo1>().vulnerable)
            {
                other.GetComponent<ContenedorEnemigo1>().vulnerable = false;*/
                switch (attackType)
                {
                    case 0:
                        //Debug.Log("Enemy hitted with headbut");
                        //attacker.DealDamage(other.gameObject, 1);
                        GameManager.Instance.chickenEnemyTakeDamage(other.GetComponent<ContenedorEnemigo1>().gameObject, 1);
                        break;
                    case 1:
                        //Debug.Log("Enemy hitted with chicken");
                        //attacker.DealDamage(other.gameObject, 2);
                        GameManager.Instance.chickenEnemyTakeDamage(other.GetComponent<ContenedorEnemigo1>().gameObject, 2);
                        break;
                }
            //}

           // StartCoroutine(DesactivarInvulnerabilidad(other.GetComponent<ContenedorEnemigo1>()));
        }
    }

    IEnumerator DesactivarInvulnerabilidad(ContenedorEnemigo1 enemigo)
    {
        yield return new WaitForSeconds(2f);
        //enemigo.vulnerable = true;
    }

}
