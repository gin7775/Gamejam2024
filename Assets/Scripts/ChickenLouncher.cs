using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenLouncher : MonoBehaviour
{
    [SerializeField] int ChickenType = 0;
    [SerializeField] GameObject[] proyectiles = null;
    [SerializeField] float proyectileForce = 10;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot(ChickenType);
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
                proyectile = Instantiate(proyectiles[0], this.transform);
                proyectile.GetComponent<Rigidbody>().AddForce(Vector3.forward * proyectileForce);
                break;
        }
    }
}
