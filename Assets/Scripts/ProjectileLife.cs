using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLife : MonoBehaviour
{
    [SerializeField] int Life = 5;
    void Start()
    {
        SetObjectLifeTime();
    }

    void SetObjectLifeTime()
    {
        StartCoroutine(ObjectLife());
    }
    IEnumerator ObjectLife()
    {
        yield return new WaitForSeconds(Life);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("name: " + other.gameObject.name);
        // Debug.Log("tag: " + other.gameObject.tag);
        if (other.gameObject.CompareTag("Enemy"))
        {
            /*switch (chickenType)
            {
                case 0:
                    Debug.Log("Enemy hitted with headbut");
                    DealDamage(other.gameObject, 1);
                    break;
                case 1:*/
                    Debug.Log("Enemy hitted with chicken");
                    DealDamage(other.gameObject, 2);
                    /*break;

            }*/
        }
    }

    private void DealDamage(GameObject objetive, int damage)
    {
        //objetive.GetComponent<ContenedorEnemigo1>().ReciveDamage(damage);
        GameManager.Instance.chickenEnemyTakeDamage(objetive, damage);
    }

}
