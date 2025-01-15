using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddImpulse : MonoBehaviour
{
    private Rigidbody rb;
    //public float impulse = 5f;
    public float drag = 0.1f;
    public float angularDrag = 0.1f;

    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        rb.drag = drag;
        rb.angularDrag = drag;
        //AddForce();
    }

    public void AddForce()
    {
        // Aplicar impulso a la bola
        //rb.AddForce(transform.forward * impulse, ForceMode.Impulse);
    }

}