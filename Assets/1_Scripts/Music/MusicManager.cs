using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class MusicManager : MonoBehaviour
{
    //Themes
    [SerializeField] AudioSource[] mainTheme;
    [SerializeField] AudioSource transitionTheme;
    [SerializeField] AudioSource sirena;

    bool reproduciendoMusic;
    //CacareoPollo
    [SerializeField] AudioSource sonidoPolloJugador;
    [SerializeField] AudioSource recogerPolloSuelo;
    [SerializeField] AudioSource[] sonidosPollosEnemigos;

    //SoundFX
    [SerializeField] AudioSource polloJugadorMuerto;


    [SerializeField] AudioSource recibirDañoJugador;
    [SerializeField] AudioSource disparoPolloJugador;


    [SerializeField] AudioSource explosionPollo;
    [SerializeField] AudioSource tickingPollo;

    [SerializeField] AudioSource clicPollo;

    [SerializeField] AudioSource comenzarOleadaFX;



    public static MusicManager Instance { get; private set; }


    int oleadaAnterior, oleadaSiguiente;
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

        oleadaAnterior = mainTheme.Length -1;
        oleadaSiguiente = 0;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{

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

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    StartCoroutine(ChangeRaidTheme(0, 0));
        //}
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    StartCoroutine(ChangeRaidTheme(0, 1));
        //}
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    StartCoroutine(ChangeRaidTheme(1, 2));
        //}





    }

    //public IEnumerator ChangeRaidTheme(int oleadaAnterior, int oleadaSiguiente)
    //{
    //    if (oleadaSiguiente < mainTheme.Length)
    //    {

    //       if(oleadaSiguiente >= mainTheme.Length)
    //        {
    //            oleadaSiguiente = 3;
    //        }
    //        transitionTheme.Play();
    //        yield return new WaitForSeconds(1f);
    //        sirena.Play();
    //        mainTheme[oleadaAnterior].Stop();

    //        try
    //        {
    //            sonidosPollosEnemigos[0].Stop();
    //            sonidosPollosEnemigos[1].Stop();
    //        } catch (System.Exception) {}

    //        yield return new WaitForSeconds(3f);
    //        Play_FX_StartRound();
    //        yield return new WaitForSeconds(1f);
    //        mainTheme[oleadaSiguiente].Play();

    //        try
    //        {
    //            sonidosPollosEnemigos[0].Play();
    //            sonidosPollosEnemigos[1].Play();
    //        }
    //        catch (System.Exception) { }

    //        transitionTheme.Stop();
    //        yield return new WaitForSeconds(1f);


    //    }
    //    else
    //    {

    //        transitionTheme.Play();
    //        yield return new WaitForSeconds(1f);
    //        sirena.Play();
    //        StopAllMusic(); 
    //        yield return new WaitForSeconds(3f);
    //        Play_FX_StartRound();
    //        yield return new WaitForSeconds(4f);
    //        mainTheme[2].Play();
    //        transitionTheme.Stop();
    //        yield return new WaitForSeconds(1f);

    //    }

    //}
    //public void FX_ActivarCorutina(int oleadaAnterior, int oleadaSiguiente)
    //{
    //    StartCoroutine(ChangeRaidTheme(oleadaAnterior,oleadaSiguiente));
    //}

    public IEnumerator ChangeRaidTheme()
    {

        transitionTheme.Play();
        yield return new WaitForSeconds(1f);
        sirena.Play();
        mainTheme[oleadaAnterior].Stop();
        try
        {
            sonidosPollosEnemigos[0].Stop();
            sonidosPollosEnemigos[1].Stop();
        }
        catch (System.Exception) { }

        yield return new WaitForSeconds(3f);
        Play_FX_StartRound();
        yield return new WaitForSeconds(1f);

        mainTheme[oleadaSiguiente].Play();
        try
        {
            sonidosPollosEnemigos[0].Play();
            sonidosPollosEnemigos[1].Play();
        }
        catch (System.Exception) { }

        transitionTheme.Stop();
        yield return new WaitForSeconds(1f);
        oleadaAnterior = oleadaSiguiente;
        oleadaSiguiente++;

        if (oleadaSiguiente >= mainTheme.Length)
        {
            oleadaSiguiente = 0;
            oleadaAnterior = mainTheme.Length - 1;
        }



    }

    public void FX_ActivarCorutina()
    {
        StartCoroutine(ChangeRaidTheme());
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
        for (int i = 0; i < mainTheme.Length; i++)
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
        // tickingPollo.Play();
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

    public void Play_FX_PLayer_RecibirDaño()         //Sonido de dolor del Jugador
    {
        recibirDañoJugador.Play();
    }

    public void Play_FX_PLayer_DispararPollo()         //Sonido de disparo del Jugador
    {
        disparoPolloJugador.Play();
    }







}
