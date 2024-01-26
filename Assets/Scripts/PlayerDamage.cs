using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public int vidas,totalLife;
    private void Start()
    {
        vidas = totalLife;
    }

    public void TakeDamage()
    {
        vidas--;
        if (vidas <= 0)
        {
            //inserte aqui muerte del jugador
        }
    }



}
