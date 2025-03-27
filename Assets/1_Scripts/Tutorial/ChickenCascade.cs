using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenCascade : MonoBehaviour
{

    int height = 10;


    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            //// Activar ragdoll (si tiene el script correspondiente)
            //RagdollController ragdoll = collision.gameObject.GetComponent<RagdollController>();
            //if (ragdoll != null)
            //{
            //    ragdoll.EnableRagdoll();
            //}

            collision.gameObject.transform.position = new Vector3(
                collision.gameObject.transform.position.x,
                collision.gameObject.transform.position.y + height,
                collision.gameObject.transform.position.z
            );

            //// Desactivar ragdoll
            //if (ragdoll != null)
            //{
            //    ragdoll.DisableRagdoll();
            //}
        }


    }
}
