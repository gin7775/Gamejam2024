using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UIElements;

public class ChickenLouncher : MonoBehaviour
{
    [SerializeField] int ChickenType = 0;
    [SerializeField] GameObject[] proyectiles = null;
    [SerializeField] float proyectileForce = 10;

    Vector3 raycastPosition;
    Vector3 raycastObjetive;

    private void Update()
    {
        RotateRay();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Shoot(ChickenType);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Attack(ChickenType);
        }
    }

    public void RotateRay()
    {
        float angle = 90;
        bool angleReached = false;
        int reyDistance = 5;
        int maxAngle = 90;


        float VelociadDeRecorrido = 300f * 4;
        if (angleReached == true)
        {
            if (angle >= maxAngle)
            {
                angle = maxAngle;
                angleReached = true;

            }
            else
            {
                angle += Time.deltaTime * VelociadDeRecorrido;
            }
        }
        if (angleReached == false)
        {
            if (angle <= -maxAngle)
            {
                angle = -maxAngle;
                angleReached = false;
            }
            else
            {
                angle -= Time.deltaTime * VelociadDeRecorrido;
            }
        }


        // actualizar el vector de dirección rotado
        Vector3 rayDirection = transform.forward * reyDistance; // dirección original del rayo
        Quaternion rotation = Quaternion.Euler(0f, angle, 0f); // crea una rotación en el eje Y
        rayDirection = rotation * rayDirection; // aplica la rotación al vector de dirección del rayo

        Debug.DrawRay(transform.position, rayDirection, Color.green); // dibuja el rayo rotado

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
                break;
        }
    }

    void Attack(int AmmoType)
    {
        switch (AmmoType)
        {
            case 0:
                Debug.Log("Got no chickens");
                break;
            case 1:
                Debug.Log("Attacking");

                raycastObjetive = transform.forward;
                raycastObjetive.x += 1;
                Ray ray = new Ray(transform.position, raycastObjetive);

                RaycastHit hitted;
                raycastPosition = transform.position;

                Debug.DrawRay(raycastPosition, raycastObjetive * 5, Color.green);

                if (Physics.Raycast(ray, out hitted, 5) && hitted.transform.tag == "Rock" || Physics.Raycast(ray, out hitted, 5) && hitted.transform.tag == "Plant")
                {
                    Debug.DrawRay(transform.position, raycastObjetive * 5, Color.red);

                }

                break;

            case 2:
                
                break;
        }
    }
}
