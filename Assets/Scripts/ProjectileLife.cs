using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLife : MonoBehaviour
{
    [SerializeField] int Life = 5;
    [SerializeField] int effectLife = 2;
    [SerializeField] Collider[] colliders;
    [SerializeField] GameObject[] ragdolls;
    [SerializeField] bool isCodeExecuting = false;
    [SerializeField] public GameObject[] chickensToDie;
    [SerializeField] public float radius = 5f;

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
        GameManager.Instance.ChickenEnemyTakeDamage(objetive, damage);
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
                    GameManager.Instance.ChickenEnemyTakeDamage(chicken, 99);
                }
            }

            isCodeExecuting = false;
        }

        StartCoroutine(ForceToChickens());
        Destroy(this.gameObject);
    }


    IEnumerator ForceToChickens()
    {
        new WaitForSeconds(0.5f);
        List<GameObject> listaChickens = new List<GameObject>(GameObject.FindGameObjectsWithTag("Corpse"));

        for (int i = 0; i < listaChickens.Count; i++)
        {
            float dist = Vector3.Distance(this.gameObject.transform.position, listaChickens[i].transform.position);
            if (dist <= 100)
            {
                GameObject bodyOne = listaChickens[i].gameObject.transform.GetChild(0).transform.GetChild(0).gameObject;
                GameObject bodyTwo = bodyOne.gameObject.transform.GetChild(0).gameObject;
                GameObject bodyNeck = bodyTwo.gameObject.transform.GetChild(2).gameObject;
                GameObject bodyHead = bodyNeck.gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;

                Collider[] colliders = Physics.OverlapSphere(this.transform.position, 100);

                foreach (Collider collider in colliders)
                {
                    bodyOne.GetComponent<CapsuleCollider>().enabled = false;
                    bodyTwo.GetComponent<CapsuleCollider>().enabled = false;
                    bodyNeck.GetComponent<CapsuleCollider>().enabled = false;
                    bodyHead.GetComponent<CapsuleCollider>().enabled = false;

                    bodyOne.GetComponent<Rigidbody>().AddExplosionForce(25.0f, this.gameObject.transform.position, 10, 25.0f);
                    bodyTwo.GetComponent<Rigidbody>().AddExplosionForce(25.0f, this.gameObject.transform.position, 10, 25.0f);
                    bodyNeck.GetComponent<Rigidbody>().AddExplosionForce(25.0f, this.gameObject.transform.position, 10, 25.0f);
                    bodyHead.GetComponent<Rigidbody>().AddExplosionForce(25.0f, this.gameObject.transform.position, 10, 25.0f);

                    bodyOne.GetComponent<CapsuleCollider>().enabled = true;
                    bodyTwo.GetComponent<CapsuleCollider>().enabled = true;
                    bodyNeck.GetComponent<CapsuleCollider>().enabled = true;
                    bodyHead.GetComponent<CapsuleCollider>().enabled = true;
                }
            }
        }

        yield return null;
    }

}
