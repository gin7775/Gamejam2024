using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBig: MonoBehaviour
{

    [Header("Duración de la vida del objeto")]
    [SerializeField] private float lifeTime = 5f;

    [Header("Configuración de daño")]
    [SerializeField] private int effectLife = 2;
    [SerializeField] private Collider[] colliders;
    [SerializeField] private GameObject[] ragdolls;

    // … resto de tus campos …

    private void Start()
    {
        // Destruye este GameObject automáticamente pasados lifeTime segundos
        Destroy(gameObject, lifeTime);
    }


    //[SerializeField] int Life = 5;
    //[SerializeField] int effectLife = 2;
    //[SerializeField] Collider[] colliders;
    //[SerializeField] GameObject[] ragdolls;

    ////private bool isCodeExecuting = false;
    //public GameObject[] chickensToDie;
    //public float radius = 5f;



    //void Start()
    //{
    //    SetObjectLifeTime();
    //    StartCoroutine(SetRagdoll());
    //}

    //void SetObjectLifeTime()
    //{
    //    StartCoroutine(ObjectEffectLife());
    //}

    //IEnumerator ObjectEffectLife()
    //{
    //    yield return new WaitForSeconds(effectLife);

    //    if(colliders != null)
    //    {
    //        foreach(Collider col in colliders)
    //        {
    //            col.enabled = false;
    //        }
    //    }

    //    StartCoroutine(ObjectLife());
    //}

    //IEnumerator ObjectLife()
    //{
    //    yield return new WaitForSeconds(Life);
    //    Destroy(gameObject);
    //}

    //IEnumerator SetRagdoll()
    //{
    //    yield return new WaitForSeconds(0.5f);

    //    foreach(GameObject obj in ragdolls)
    //        obj.SetActive(true);
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Impacto");
            DealDamage(other.gameObject, 2);
        }
    }

    private void DealDamage(GameObject objetive, int damage)
    {
        //objetive.GetComponent<ContenedorEnemigo1>().ReciveDamage(damage);
        GameManager.Instance.ChickenEnemyTakeDamage(objetive, damage, false);
    }

}
