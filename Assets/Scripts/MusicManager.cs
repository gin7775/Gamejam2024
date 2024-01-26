using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    //Themes
    [SerializeField] AudioSource mainTheme;
    [SerializeField] AudioSource menuTheme;


    //CacareoPollo
    [SerializeField] AudioSource sonidoPolloJugador;

    //SoundFX
    [SerializeField] AudioSource ataquePollo;
    [SerializeField] AudioSource polloMuerto;

    public void PlayMaintheme()
    {
        mainTheme.Play();
    }
    public void playMenuTheme()
    {
        menuTheme.Play();
    }
    public void StopMainTheme()
    {
        mainTheme.Stop();
    }
    public void StopMenuTheme()
    {
        menuTheme.Stop();
    }

    public void Sound_CacareoPollo()
    {
        sonidoPolloJugador.Play();
    }

    public void Sound_AtaquePollo()
    {
        ataquePollo.Play();
    }

    public void Sound_PolloMuerto()
    {
        polloMuerto.Play();
    }

    



}
