using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTouchWater : MonoBehaviour
{
    public Transform respawn;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");


        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Respawn");
            collision.gameObject.transform.position = respawn.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");


        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Respawn");
            //other.gameObject.transform.position = respawn.position;
            other.GetComponent<CharacterController>().enabled = false;
            other.gameObject.GetComponent<Rigidbody>().position = respawn.position;
            other.GetComponent<CharacterController>().enabled = true;
        }
    }



}
