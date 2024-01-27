using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{

    [SerializeField] AudioSource[] sonidoPolloEnemigo;
    [SerializeField] AudioSource polloMuerto;

    int indexElegido;

    // Start is called before the first frame update
    void Start()
    {
        indexElegido = Random.Range(0, sonidoPolloEnemigo.Length);
        sonidoPolloEnemigo[indexElegido].Play();
    }

    // Update is called once per frame
  
    public void Stop_CacareoJugador() //Frena el Cacareo
    {
        sonidoPolloEnemigo[indexElegido].Stop();

    }

    public void Play_Enemy_Sound_PolloMuerto() // PolloMuerto
    {
        polloMuerto.Play();
    }

}
