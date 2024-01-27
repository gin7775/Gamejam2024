using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParticles : MonoBehaviour
{
    // particula & si hubieran mas (ejemplo plumas + humo)
    public GameObject particle;
    public GameObject secondaryParticle;
    // Dos funciones de Spawn
    public void SpawnParticle()
    {
        Instantiate(particle,this.transform.position, Quaternion.identity);
    }
    public void SpawnSecondaryParticle()
    {
        Instantiate (secondaryParticle, this.transform.position, Quaternion.identity);
    }


    public void SpawnBothParticles()
    {
        if (particle != null)
        {
            SpawnParticle();
            if (secondaryParticle != null)
            {
                SpawnSecondaryParticle();
            }
        }


        
        
    }
}
