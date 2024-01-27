using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    //Themes
    [SerializeField] AudioSource mainTheme;


    //CacareoPollo
    [SerializeField] AudioSource sonidoPolloJugador;
    //[SerializeField] AudioSource[] sonidosPollosEnemigos;

    //SoundFX
    [SerializeField] AudioSource ataquePollo;
    [SerializeField] AudioSource polloMuerto;

    [SerializeField] AudioSource explosionPollo;
    [SerializeField] AudioSource tickingPollo;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
          Play_Sound_AtaquePollo();   
        }
    }

    //Menu
    public void PlayMaintheme()
    {
        mainTheme.Play();
    }
  
    public void StopMainTheme()
    {
        mainTheme.Stop();
    }

    public void Play_Sound_PlayerCacareoPollo()
    {
        sonidoPolloJugador.Play();
    }

    public void Play_Sound_AtaquePollo()
    {
        ataquePollo.Play();
    }

    public void Sound_PolloMuerto()
    {
        polloMuerto.Play();
    }

    public void Sound_ExplosionPollo()
    {
        explosionPollo.Play();
    }

    public void Stop_ExplosionPollo()
    {
        explosionPollo.Stop();
    }

    public void Sound_TickingPollo()
    {
        tickingPollo.Play();
    }

    





}
