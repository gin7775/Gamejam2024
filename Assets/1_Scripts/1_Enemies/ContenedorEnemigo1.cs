using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ContenedorEnemigo1 : MonoBehaviour
{
    public Animator animEnemy;
    public GameObject playerReference;
    public int enemyTipe;
    public float distanceToEnemy;
    public Transform destination;
    [SerializeField] public int lifes = 3;
    private CinemachineImpulseSource cinemachineImpulseSource;
    public float speed;
    public float distanceToAttack = 1f;
    public GameObject corpse;
    public bool canDamage;

    public void Start()
    {
        canDamage = true;
        cinemachineImpulseSource = this.GetComponent<CinemachineImpulseSource>();
        playerReference = GameObject.FindGameObjectWithTag("Player");
    }

    public void PolloMansy()
    {
        //Debug.Log("I manifest the greatest of phoes! A chicken! WORSE... A Deadly Chicken... it's dead... oh...");
        GameObject corpseTimeLife = GameObject.Instantiate(corpse, transform.position, Quaternion.identity);
        CorpseManager.Instance.addCorpseTimeLife(corpseTimeLife);
    }

    /*
    public void ReciveDamage(int damage)
    {
        if (canDamage == true)
        {
            cinemachineImpulseSource.GenerateImpulse();
            lifes -= damage;
            //GameManager.Instance.chickenEnemyTakeDamage(damage);
            canDamage = false;
            StartCoroutine(FrameFreeze(0.1f));
            //StartCoroutine(Contador());
            //GameManager.Instance.enemyCount--;
        }
       
    }
    
    private IEnumerator FrameFreeze(float duration)
    {
        Time.timeScale = 0f;
        //Debug.Log("Parao");
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
    }

    public IEnumerator Contador()
    {
        yield return new WaitForSeconds(1);
        canDamage = true;
    }*/

}
