using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animaciones : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MovementAnimation()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Walking");
    }

    public void DeathAnimation()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Die");
    }

    public void AttackAnimation()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Attack");
    }

}
