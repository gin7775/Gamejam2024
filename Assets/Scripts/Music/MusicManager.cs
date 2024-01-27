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
    [SerializeField] AudioSource recogerPolloSuelo;
    //[SerializeField] AudioSource[] sonidosPollosEnemigos;

    //SoundFX
    [SerializeField] AudioSource polloJugadorMuerto;
    

    [SerializeField] AudioSource explosionPollo;
    [SerializeField] AudioSource tickingPollo;

    [SerializeField] AudioSource clicPollo;

    [SerializeField] AudioSource comenzarOleadaFX;

    public static MusicManager Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartCoroutine(ChangeRaidTheme(0,1));
        //}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    //Play_FX_Jugador_PolloMuerto();
        //}

        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    Play_FX_RecogerPollo();
        //}
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    Play_FX_StartRound();
        //}

    }

    public IEnumerator ChangeRaidTheme(int oleadaAnterior, int oleadaSiguiente)
    {
        if(oleadaSiguiente < mainTheme.Length)
        {
            transitionTheme.Play();
            yield return new WaitForSeconds(1f);
            mainTheme[oleadaAnterior].Stop();
            yield return new WaitForSeconds(3f);
            Play_FX_StartRound();
            yield return new WaitForSeconds(1f);
            mainTheme[oleadaSiguiente].Play();
            yield return new WaitForSeconds(1f);
       

        }

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

    public void StopAllMusic()
    {
        for(int i=0; i<mainTheme.Length; i++)
        {
            mainTheme[i].Stop();
        }
    }
    


    //FX
    public void Play_FX_Player_CacareoPollo() //Cacareo Jugador
    {
        sonidoPolloJugador.Play();
    }
 
    public void Play_FX_Player_PolloMuerto()        // Pollo Muerto
    {
        polloJugadorMuerto.Play();
    }

    public void Play_FX_ExplosionPollo()          //Explosion Pollo
    {
        explosionPollo.Play();
    }

    public void Play_FX_TickingPollo()          //Tic tac pollo
    {
        tickingPollo.Play();
    }
    public void Play_FX_ClicPollo()         //Clic Pollo Menu Boton Jugar
    {
        clicPollo.Play();
    }
    public void Play_FX_RecogerPollo()         //Recoger Pollo
    {
        recogerPolloSuelo.Play();
    }

    public void Play_FX_StartRound()         //Quiquiriquí
    {
        comenzarOleadaFX.Play();
    }








}
