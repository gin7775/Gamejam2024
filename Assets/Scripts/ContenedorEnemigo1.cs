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

    public void Start()
    {
        cinemachineImpulseSource = this.GetComponent<CinemachineImpulseSource>();
         playerReference = GameObject.FindGameObjectWithTag("Player");
    }
    public void ReciveDamage(int damage)
    {
        cinemachineImpulseSource.GenerateImpulse();
        lifes -= damage;
        PolloMansy();
        StartCoroutine(FrameFreeze(0.1f));

        if (lifes <= 0)
        {
            //GameManager.Instance.chikenEnemyDeath(gameObject);
            //Destroy(gameObject);
        }
    }

    private IEnumerator FrameFreeze(float duration)
    {
        Time.timeScale = 0f;
        Debug.Log("Parao");
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
    }
    public void PolloMansy()
    {
        GameObject toInstantiate = GameObject.Instantiate(corpse, transform.position, Quaternion.identity);
        
    }
   

}
