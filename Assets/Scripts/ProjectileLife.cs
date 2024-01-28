using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLife : MonoBehaviour
{
    [SerializeField] int Life = 5;
    [SerializeField] int effectLife = 2;
    [SerializeField] Collider[] colliders;
    [SerializeField] GameObject[] ragdolls;

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

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("Enemy hitted with chicken");
            DealDamage(other.gameObject, 2);
        }
    }

    private void DealDamage(GameObject objetive, int damage)
    {
        //objetive.GetComponent<ContenedorEnemigo1>().ReciveDamage(damage);
        GameManager.Instance.chickenEnemyTakeDamage(objetive, damage);
    }

    IEnumerator SetRagdoll()
    {
        yield return new WaitForSeconds(0.5f);
        foreach(GameObject obj in ragdolls)
        {
            obj.SetActive(true);
        }
    }

}
