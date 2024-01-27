using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ContenedorEnemigo1 : MonoBehaviour
{
    public Animator animEnemy;
    
    public float distanceToEnemy;
    public Transform destination;
    [SerializeField] public int lifes = 3;

    public float speed;
    public float distanceToAttack = 1f;

    public void ReciveDamage(int damage)
    {
        lifes -= damage;

        if(lifes <= 0)
        {
            //GameManager.Instance.chikenEnemyDeath(gameObject);
            Destroy(gameObject);
        }
    }

    //public void CheckDistance()
    //{
    //    GameObject player = GameObject.FindGameObjectWithTag("Player");

    //    if (Vector3.Distance(transform.position, player.transform.position) <= distanceToAttack)
    //    {
    //        player.GetComponent<ChickenLouncher>().ReciveDamage(1);

    //    }
    //}

}
