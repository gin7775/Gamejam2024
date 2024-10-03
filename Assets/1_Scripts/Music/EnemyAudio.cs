using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{

    [SerializeField] AudioSource[] sonidoPolloEnemigo;
    [SerializeField] AudioSource polloMuerto;
    [SerializeField] AudioSource tickingPollo;

    [SerializeField] bool isExplosive;

    int indexElegido;

    // Start is called before the first frame update
    void Start()
    {
        indexElegido = Random.Range(0, sonidoPolloEnemigo.Length);
        sonidoPolloEnemigo[indexElegido].Play();
        if (isExplosive)
        {
            tickingPollo.Play();
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Stop_CacareoJugador();
        //    Play_Enemy_FX_PolloMuerto();
        //}
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    Play_Enemy_FX_TickingPollo();
        //}
    }

    public void Stop_CacareoJugador() //Frena el Cacareo
    {
        sonidoPolloEnemigo[indexElegido].Stop();

    }

    public void Play_Enemy_FX_PolloMuerto() // PolloMuerto
    {
        polloMuerto.Play();
    }

    public void Play_Enemy_FX_TickingPollo()
    {
        tickingPollo.Play();
    }

}
