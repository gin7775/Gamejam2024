using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject[] pollosToSpawn;
    [SerializeField] Vector3 spawnPosition;
    [SerializeField] int randomIterastor, probabilityIterator;
    // Tiempo por el que espera para generar enemigos por segundo
    [SerializeField] float timeWave;
    // Tiempo por el que espera para generar enemigos por segundo
    [SerializeField] float timeGeneration;
    public int score = 0;
    // Numero total de enemigos en la ronda
    public int enemyNumber;
    // Numero de enemigos por oleada inicial
    public int enemyInitial;
    // Numero de enemigos que quedan en la ronda
    public int enemyCount;
    // Numero de ronda totales
    public int waveNumber;
    // Numero de ronda actual
    public int waveCurrent;
    // Nivel de dificultad -- No se usa
    public int dificultiLevel;
    // Nivel de dificultad -- No se usa
    [SerializeField] Canvas canvasRound;
    [SerializeField] Animator canvasAnimator;
    [SerializeField] GameObject textMesh;
    [SerializeField] GameObject vfxHitEffect;
    [SerializeField] GameObject vfxHitWaveEffect;
    [SerializeField] GameObject SmokeEffect;
    [SerializeField] CinemachineImpulseSource cinemachineImpulseSource;
    [SerializeField] int numMaxWave1;
    [SerializeField] int numMaxWave2;
    [SerializeField] int numMaxWave3;
    [SerializeField] bool siguiente;
    [SerializeField] MusicManager musicManager;
    public GameObject scoreText;
    public float limiteXNegativo, LimiteXPositivo, limiteZNegativo, LimiteZPositivo;
    public bool paused = false;
    public GameObject pausemenu;
    //Music Variables
    public AudioMixer myMixer;
    public Slider musicSlider;
    public Slider SFXSlider;

    private void Awake()
    {
        // Si hay una instancia y no es esta, borrala.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        timeGeneration = 3f;
        timeWave = 6f;
        enemyInitial = 5;
        enemyNumber = 0;
        enemyCount = 0;
        waveNumber = 3;
        numMaxWave1 = 20;
        numMaxWave2 = 50;
        numMaxWave3 = 100;
        siguiente = false;
        UpdateWave();
    }

    public void UpdateWave()
    {
        enemyCount = 0;
        waveCurrent++;

        if (waveCurrent <= waveNumber)
        {
            textMesh.GetComponent<TextMeshProUGUI>().text = "Round " + waveCurrent;

            if (waveCurrent <= 1)
            {
                dificultiLevel = 1;
                timeGeneration = 2.5f;
            }
            else if (waveCurrent == 2)
            {
                dificultiLevel = 2;
                timeGeneration = 1.5f;
            }
            else
            {
                dificultiLevel = 3;
                timeGeneration = 0.85f;
            }

            StartCoroutine(StartWave(timeWave, enemyInitial * waveCurrent));
        }
        else
        {
            dificultiLevel = 4;
            timeGeneration = 0.75f;
            StartCoroutine(StartWave(timeWave, enemyInitial * 4));
        }
    }

    public void ChikenGenerator()
    {
        probabilityIterator = Random.Range(0, 100);

        if (probabilityIterator >= 0 && probabilityIterator <= 60)
        {
            randomIterastor = 0;
        }
        else if (probabilityIterator > 60 && probabilityIterator <= 75)
        {
            randomIterastor = 1;
        }
        else if (probabilityIterator > 75 && probabilityIterator <= 90)
        {
            randomIterastor = 2;
        }
        else if (probabilityIterator > 90 && probabilityIterator <= 100)
        {
            randomIterastor = 3;
        }

        spawnPosition = new Vector3(Random.Range(limiteXNegativo, LimiteXPositivo), 1.2f, Random.Range(limiteZNegativo, LimiteZPositivo));
        Instantiate(pollosToSpawn[randomIterastor], spawnPosition, Quaternion.identity);
        //enemyCount++;
    }

    public void ChickenEnemyTakeDamage(GameObject enemy, int damage)
    {
        int auxLife = 0;
        // Spawn particula de hit
        Instantiate(vfxHitWaveEffect, enemy.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        if (enemy != null)
        {
            if (enemy.GetComponent<ContenedorEnemigo1>() != null)
            {
                auxLife = enemy.GetComponent<ContenedorEnemigo1>().lifes -= damage;
            }
        }

        if (auxLife <= 0)
        {
            cinemachineImpulseSource = enemy.gameObject.GetComponent<CinemachineImpulseSource>();
            cinemachineImpulseSource.GenerateImpulse();
            Instantiate(vfxHitEffect, enemy.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            StartCoroutine(FrameFreeze(0.03f));
            score++;
            enemy.GetComponent<ContenedorEnemigo1>().PolloMansy();
            Destroy(enemy);
            EnemyDeath();
        }
    }

    public void EnemyDeath()
    {
        enemyCount++;

        //AUDIO: Ver si funciona en lso enemigos sino, se pone aquí
        musicManager.Play_FX_ExplosionPollo();

        if (enemyCount >= (enemyInitial * waveCurrent + enemyNumber) && siguiente)
        {
            UpdateWave();
        }
    }

    IEnumerator StartWave(float seconds, int totalEnemies)
    {
        // Activa el canvas con la animacion de la ronda actual
        canvasRound.gameObject.SetActive(true);
        musicManager.FX_ActivarCorutina(waveCurrent - 1, waveCurrent);

        yield return new WaitForSeconds(seconds - 2f);

        // Hace que el texto de ronda se vaya a la derecha
        if (canvasAnimator != null)
        {
            canvasAnimator.SetTrigger("Change");
        }

        yield return new WaitForSeconds(2f);

        // Desactiva el canvas con la animacion de la ronda actual
        canvasRound.gameObject.SetActive(false);

        // Hace que el texto de ronda vuelva al estado normal   
        if (canvasAnimator != null)
        {
            canvasAnimator.SetTrigger("Change");
        }

        // Genera pollos segun la variable totalEnemies
        for (int i = 0; i < totalEnemies; i++)
        {
            ChikenGenerator();
        }

        // Genera poco a poco pollos segun la variable totalEnemies
        StartCoroutine(ChikenWaitSpawner());
    }

    IEnumerator ChikenWaitSpawner()
    {
        siguiente = false;

        enemyNumber = waveCurrent <= 1 ? numMaxWave1 - enemyInitial * waveCurrent : 0;
        enemyNumber = waveCurrent == 2 ? numMaxWave2 - enemyInitial * waveCurrent : enemyNumber;
        enemyNumber = waveCurrent == 3 ? numMaxWave3 - enemyInitial * waveCurrent : enemyNumber;
        enemyNumber = waveCurrent >= 4 ? numMaxWave3 - enemyInitial * waveCurrent : enemyNumber;

        for (int i = 0; i < enemyNumber; i++)
        {
            yield return new WaitForSeconds(timeGeneration);
            ChikenGenerator();
        }

        siguiente = true;
    }

    private IEnumerator FrameFreeze(float duration)
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
    }

    public void ActivarFXMuerte()
    {

    }

    //Menu pausa
    public void exit_pause_menu()
    {
        Time.timeScale = 1;
        paused = false;
        pausemenu.SetActive(false);
    }

    public void pause()
    {
        if (paused)
        {
            exit_pause_menu();
        }

        if (paused == false)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (currentSceneName != "MenuPrincipal")
            {
                paused = true;
                pausemenu.gameObject.SetActive(true);
                Time.timeScale = 0;
            }

            if (currentSceneName == "MenuPrincipal")
            {
                // Estás en la Escena principal
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Back_to_Main_Menu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("0_MenuPrincipal");
    }

    // Music Manager
    public void SetMusicVolume()
    {
        // Debug.Log("Music modified");
        float volume = musicSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("FX", Mathf.Log10(volume) * 20);
    }

}
