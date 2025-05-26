using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class MusicManager : MonoBehaviour
{
    // === MUSIC TRACKS ===
    [SerializeField] private AudioClip[] mainThemes;
    [SerializeField] private AudioClip transitionTheme;
    [SerializeField] private AudioClip sirena;

    // === FX SFX ===
    [SerializeField] private AudioClip sonidoPolloJugador;
    [SerializeField] private AudioClip recogerPolloSuelo;
    [SerializeField] private AudioClip[] sonidosPollosEnemigos;

    [SerializeField] private AudioClip polloJugadorMuerto;
    [SerializeField] private AudioClip recibirDanoJugador;
    [SerializeField] private AudioClip disparoPolloJugador;
    [SerializeField] private AudioClip explosionPollo;
    [SerializeField] private AudioClip tickingPollo;
    [SerializeField] private AudioClip clicPollo;
    [SerializeField] private AudioClip comenzarOleadaFX;
    private bool _isTransitioning = false;
    public static MusicManager Instance { get; private set; }

    private int oleadaAnterior;
    private int oleadaSiguiente;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        oleadaAnterior = mainThemes.Length - 1;
        oleadaSiguiente = 0;
    }

    // === CAMBIO DE OLEADA ===

    public void FX_ActivarCorutina()
    {
        if (_isTransitioning) return;   // si ya estamos en transición, no hacemos nada
        _isTransitioning = true;
        StartCoroutine(ChangeRaidTheme());
    }

    public IEnumerator ChangeRaidTheme()
    {
        // 1. Parar música actual para evitar solapamientos
        MMSoundManager.Instance.StopTrack(MMSoundManager.MMSoundManagerTracks.Music);

        // 2. Sonido transición
        MMSoundManagerSoundPlayEvent.Trigger(transitionTheme, GetSfxOptions());

        yield return new WaitForSeconds(1f);

        // 3. Sirena
        MMSoundManagerSoundPlayEvent.Trigger(sirena, GetSfxOptions());

        // 4. Parar sonidos enemigos anteriores
        foreach (var s in sonidosPollosEnemigos)
        {
            var src = MMSoundManager.Instance.FindByClip(s);
            if (src != null) MMSoundManager.Instance.StopSound(src);
        }

        yield return new WaitForSeconds(3f);

        // 5. Quiquiriquí del inicio
        Play_FX_StartRound();
        yield return new WaitForSeconds(1f);

        // 6. Nueva canción de oleada con fade
        MMSoundManagerPlayOptions opcionesMusica = MMSoundManagerPlayOptions.Default;
        opcionesMusica.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Music;
        opcionesMusica.Location = transform.position;
        opcionesMusica.Volume = 1f;
        opcionesMusica.Loop = true;
        opcionesMusica.Fade = true;
        opcionesMusica.FadeDuration = 1f;
        opcionesMusica.FadeInitialVolume = 0f;

        MMSoundManagerSoundPlayEvent.Trigger(mainThemes[oleadaSiguiente], opcionesMusica);

        // 7. Sonidos de enemigos en loop
        foreach (var s in sonidosPollosEnemigos)
        {
            MMSoundManagerPlayOptions opciones = MMSoundManagerPlayOptions.Default;
            opciones.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Sfx;
            opciones.Location = transform.position;
            opciones.Volume = 1f;
            opciones.Loop = true;
            MMSoundManagerSoundPlayEvent.Trigger(s, opciones);
        }

        // 8. Actualizar oleadas
        oleadaAnterior = oleadaSiguiente;
        oleadaSiguiente++;

        if (oleadaSiguiente >= mainThemes.Length)
        {
            oleadaSiguiente = 0;
            oleadaAnterior = mainThemes.Length - 1;
        }
        _isTransitioning = false;
    }

    // === FX / SFX ===

    public void Play_FX_Player_CacareoPollo()
    {
        MMSoundManagerSoundPlayEvent.Trigger(sonidoPolloJugador, GetSfxOptions());
    }

    public void Play_FX_Player_PolloMuerto()
    {
        MMSoundManagerSoundPlayEvent.Trigger(polloJugadorMuerto, GetSfxOptions());
    }

    public void Play_FX_ExplosionPollo()
    {
        MMSoundManagerSoundPlayEvent.Trigger(explosionPollo, GetSfxOptions());
    }

    public void Play_FX_TickingPollo()
    {
        MMSoundManagerSoundPlayEvent.Trigger(tickingPollo, GetSfxOptions());
    }

    public void Play_FX_ClicPollo()
    {
        MMSoundManagerSoundPlayEvent.Trigger(clicPollo, GetSfxOptions());
    }

    public void Play_FX_RecogerPollo()
    {
        MMSoundManagerSoundPlayEvent.Trigger(recogerPolloSuelo, GetSfxOptions());
    }

    public void Play_FX_StartRound()
    {
        MMSoundManagerSoundPlayEvent.Trigger(comenzarOleadaFX, GetSfxOptions());
    }

    public void Play_FX_PLayer_RecibirDano()
    {
        MMSoundManagerSoundPlayEvent.Trigger(recibirDanoJugador, GetSfxOptions());
    }

    public void Play_FX_PLayer_DispararPollo()
    {
        MMSoundManagerSoundPlayEvent.Trigger(disparoPolloJugador, GetSfxOptions());
    }

    // === UTILIDAD ===

    private MMSoundManagerPlayOptions GetSfxOptions(float volume = 1f)
    {
        MMSoundManagerPlayOptions opciones = MMSoundManagerPlayOptions.Default;
        opciones.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Sfx;
        opciones.Location = transform.position;
        opciones.Volume = volume;
        return opciones;
    }

    public void StopAllMainThemes()
    {
        foreach (var clip in mainThemes)
        {
            var src = MMSoundManager.Instance.FindByClip(clip);
            if (src != null) MMSoundManager.Instance.StopSound(src);
        }
    }
}
