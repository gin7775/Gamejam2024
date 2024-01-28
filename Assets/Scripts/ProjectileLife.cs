using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLife : MonoBehaviour
{
    [SerializeField] int Life = 5;
    [SerializeField] int effectLife = 2;
    [SerializeField] Collider[] colliders;
    [SerializeField] GameObject[] ragdolls;

    private bool isCodeExecuting = false;
    public GameObject[] chickensToDie;
    public float radius = 5f;



    void Start()
    {
        SetObjectLifeTime();
        StartCoroutine(SetRagdoll());
    }

    void SetObjectLifeTime()
    {
        StartCoroutine(ObjectEffectLife());
    }

    IEnumerator ObjectEffectLife()
    {
        yield return new WaitForSeconds(effectLife);
        if(colliders != null)
        {
            foreach(Collider col in colliders)
            {
                col.enabled = false;
            }
            
        }
        StartCoroutine(ObjectLife());
    }

    IEnumerator ObjectLife()
    {
        yield return new WaitForSeconds(Life);
        Destroy(gameObject);
    }

    IEnumerator SetRagdoll()
    {
        yield return new WaitForSeconds(0.5f);
        foreach(GameObject obj in ragdolls)
        {
            obj.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy") && this.gameObject.GetComponent<ChickenCorpse>() == null)
        {
            DealDamage(other.gameObject, 2);
        }

        else if (other.gameObject.CompareTag("Enemy") && this.gameObject.GetComponent<ChickenCorpse>().chickenType == 3)
        {
            //Debug.Log("Enemy hitted with chicken");
            DealDamage(other.gameObject, 2);
            Explosion();
        }

        
    }

    private void DealDamage(GameObject objetive, int damage)
    {
        //objetive.GetComponent<ContenedorEnemigo1>().ReciveDamage(damage);
        GameManager.Instance.chickenEnemyTakeDamage(objetive, damage);
    }

    public void Explosion()
    {
        GetComponent<SpawnParticles>().SpawnBothParticles();
        if (!isCodeExecuting)
        {
            isCodeExecuting = true;

            chickensToDie = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject chicken in chickensToDie)
            {
                if (Vector3.Distance(transform.position, chicken.transform.position) <= radius)
                {
                    GameManager.Instance.chickenEnemyTakeDamage(chicken, 99);
                }
            }

            isCodeExecuting = false;
        }

        Destroy(this.gameObject);
    }

}
