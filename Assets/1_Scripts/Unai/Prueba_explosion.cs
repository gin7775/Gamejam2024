using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prueba_explosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void he_explotado()
    {
        Debug.Log("He muerto en una explosi�n jo qu� pena, no sab�a que acabar�a tan inesperadamente. La vida es tan corta, tan r�pida... quiero seguir viviendo, no quiero morir...");
        this.gameObject.SetActive(false);
    }
}
