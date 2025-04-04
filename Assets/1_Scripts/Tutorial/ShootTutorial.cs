using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTutorial : MonoBehaviour
{
    public int ClicksNeeded = 2;
    public int ClicksDone = 0;
    public GameObject bridge1;
    public GameObject bridge2;
    public GameObject obstacle1; 
    public GameObject obstacle2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateClick()
    {
        ClicksDone++;
        if (ClicksDone == ClicksNeeded)
        {
            // Activar trigger "lever" en ambos puentes
            Animator anim1 = bridge1.GetComponent<Animator>();
            Animator anim2 = bridge2.GetComponent<Animator>();

            if (anim1 != null) anim1.SetTrigger("Lever");
            if (anim2 != null) anim2.SetTrigger("Lever");

            // Desactivar obstáculos
            obstacle1.SetActive(false);
            obstacle2.SetActive(false);
        }

    }
}
