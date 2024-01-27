using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class ChickenLouncher : MonoBehaviour
{
    public int chickenType = 0;
    [SerializeField] GameObject[] proyectiles = null;
    [SerializeField] float proyectileForce = 10;
    [SerializeField] int chickenCurrentUses = 0;
    [SerializeField] int chickenMaxUses = 3;

    [SerializeField] GameObject headBox;
    [SerializeField] GameObject swingBox;


    // Gestor de Vidas Player
    [SerializeField] int health = 3;
    [SerializeField] int maxHealth = 3;

    [SerializeField] GameObject[] eggs;

    private void Update()
    {
       
    }

    public void OnShoot(InputValue value)
    {
        Shoot(chickenType);
        Debug.Log("Dispara");
    }

    public void OnAttack(InputValue value)
    {
        Attack(chickenType);
        Debug.Log("Ataca");
    }

    public void OnPick(InputValue value)
    {
        RetrieveChicken(1);
        Debug.Log("Coge");
    }

    void RetrieveChicken(int chickenNumber)
    {
        chickenType = chickenNumber;
        if (chickenType == 3)
        {
            LifeUp(1);
        }
    }


    void LifeUp(int extraLife)
    {
        health += extraLife;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
    void ReciveDamage(int damage)
    {
        UpdateLifeUI();
        health -= damage;
        if (health <= 0)
        {
            PlayerDeath();
        }
    }

    private void UpdateLifeUI()
    {
        if (eggs != null)
        {
            eggs[health - 1].GetComponent<Animator>().SetTrigger("Break");
        }
    }

    void PlayerDeath()
    {
        Debug.Log("Ye dead!");
    }



    void UpdateWeapon()
    {
        chickenCurrentUses++;
        if (chickenCurrentUses == chickenMaxUses)
        {
            chickenType = 0;
        }
    }

    void Shoot(int AmmoType)
    {
        GameObject proyectile;

        switch (AmmoType)
        {
            case 0:
                Debug.Log("Got no chickens");
                break;
            case 1:
                Vector3 projectilePos;
                projectilePos = transform.position;
                projectilePos += transform.forward;
                Debug.Log("Lounching Chicken");
                proyectile = Instantiate(proyectiles[0], projectilePos, Quaternion.identity);
                proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);

                chickenType = 0;
                break;
        }
    }

    public void Attack(int AmmoType)
    {
        switch (AmmoType)
        {
            case 0:
                Debug.Log("Got no chickens");
                HeadBut();
                break;
            case 1:
                ChickenSwing();
                UpdateWeapon();
                break;
        }
    }

    public void HeadBut()
    {
        if (headBox != null)
        {
            Debug.Log("Headbutting");
            StartCoroutine(ActivateCollider(headBox));
        }
        else
        {
            Debug.Log("Headbut Collider is missing");
        }
    }
    public void ChickenSwing()
    {
        if (swingBox != null)
        {
            Debug.Log("Swinging a chicken");
            StartCoroutine(ActivateCollider(swingBox));
        }
        else
        {
            Debug.Log("Swing Collider is missing");
        }
    }
    IEnumerator ActivateCollider(GameObject collider)
    {
        collider.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        collider.SetActive(false);
    }

    public void DealDamage(GameObject objetive, int damage)
    {
        objetive.GetComponent<ContenedorEnemigo1>().ReciveDamage(damage);
    }
}
