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

    public void ReciveDamage(int damage)
    {
        lifes -= damage;

        if(lifes <= 0)
        {
            //GameManager.Instance.chikenEnemyDeath(gameObject);
            Destroy(gameObject);
        }
    }

}
