using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UIElements;

public class ChickenLouncher : MonoBehaviour
{
    [SerializeField] int chickenType = 0;
    [SerializeField] GameObject[] proyectiles = null;
    [SerializeField] float proyectileForce = 10;
    [SerializeField] int chickenCurrentUses = 0;
    [SerializeField] int chickenMaxUses = 3;

    [SerializeField] Collider headBox;
    [SerializeField] Collider swingBox;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Shoot(chickenType);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Attack(chickenType);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RetrieveChicken(1);
        }
    }

    void RetrieveChicken(int chickenNumber)
    {
        chickenType = chickenNumber;
    }

    void UpdateWeapon()
    {
        chickenCurrentUses++;
        if(chickenCurrentUses == chickenMaxUses)
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
                Debug.Log("Lounching Chicken");
                proyectile = Instantiate(proyectiles[0], transform);
                proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);

                chickenType = 0;
                break;
        }
    }

    void Attack(int AmmoType)
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
    IEnumerator ActivateCollider(Collider collider)
    {
        collider.enabled = true;
        yield return new WaitForSeconds(2);
        collider.enabled = false;
    }
}
