using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public int vidas,totalLife;
    public GameObject[] eggs; 
    private void Start()
    {
        vidas = totalLife;
    }

    public void TakeDamage()
    {
        eggs[vidas-1].GetComponent<Animator>().SetTrigger("Break");
        vidas--;
        if (vidas <= 0)
        {
            //inserte aqui muerte del jugador
        }
    }



}
