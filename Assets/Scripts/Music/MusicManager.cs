using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    //Themes
    [SerializeField] AudioSource[] mainTheme;
    [SerializeField] AudioSource transitionTheme;

    bool reproduciendoMusic;
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
            StartCoroutine(ChangeRaidTheme(0,1));
        }
    }

    public IEnumerator ChangeRaidTheme(int oleadaAnterior, int oleadaSiguiente)
    {
       
        transitionTheme.Play();
        yield return new WaitForSeconds(1f);
        mainTheme[oleadaSiguiente].Play();
        yield return new WaitForSeconds(1f);
        mainTheme[oleadaAnterior].Stop();


    }

    //Themes  
    public void PlayMaintheme(int track)
    {
        mainTheme[track].Play();
    }
  
    public void StopMainTheme(int track)
    {
        mainTheme[track].Stop();
    }
    


    //FX
    public void Play_Sound_PlayerCacareoPollo()
    {
        sonidoPolloJugador.Play();
    }

    public void Play_Sound_AtaquePollo()
    {
        ataquePollo.Play();
    }

    public void Play_Sound_PolloMuerto()
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
