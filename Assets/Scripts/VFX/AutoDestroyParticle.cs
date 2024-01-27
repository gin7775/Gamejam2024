using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class AutoDestroyParticle : MonoBehaviour
{
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {      
        time = time+Time.deltaTime;
       if (time >= 5)
           Destroy(this.gameObject);       
    }
}
