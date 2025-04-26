using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using MoreMountains.Feedbacks;
public class PlayerHealth : MonoBehaviour
{
    // Gestor de Vidas Player
    [Header("Player Health")]
    [SerializeField] private int currentHealth = 3; // Vida actual del jugador
    [SerializeField] private int maxHealth = 3; // Vida m�xima del jugador
    [SerializeField] private GameObject[] healthIndicators; // Indicadores de vida (antes: "eggs")
    public GameObject restartButtonFirstObject; // Objeto seleccionado al reiniciar (antes: "firstGameObjectRetry")

    // RECIBIR DA�O
    [Header("Damage System")]
    [SerializeField] private float invulnerabilityDuration = 1f; // Duraci�n de la invulnerabilidad
    private bool isInvulnerable = false; // Si el jugador est� invulnerable
    [SerializeField] private ParticleSystem damageParticleEffect; // Efecto de part�culas al recibir da�o
    public GameObject highScorePanel;
    [SerializeField] private Material playerMaterial; // Material del jugador para efectos visuales

    // Propiedades del material del jugador
    private static readonly int ColorShiftKey = Shader.PropertyToID("_Is_ColorShift");
    private static readonly int RimLightKey = Shader.PropertyToID("_RimLight");
    private static readonly int ViewShiftKey = Shader.PropertyToID("_Is_ViewShift");

    private Coroutine invulnerabilityCoroutine; // Coroutine de invulnerabilidad
    private bool isDying = false; // Si el jugador est� muriendo (antes: "muriendo")

    // Animations and Effects
    [Header("Animations and VFX")]
    private Animator animator;
    private CinemachineImpulseSource cameraShake; // Fuente de impulso para el efecto de sacudida de c�mara
    [SerializeField] private MusicManager musicManager; // Gestor de m�sica y efectos de sonido
    private GameManager gameManager; // Gestor del juego

    public MMFeedbacks damageFeedback;


    private void Start()
    {
        // Inicializaci�n de componentes
        animator = GetComponent<Animator>();
        musicManager = FindAnyObjectByType<MusicManager>();
        gameManager = FindAnyObjectByType<GameManager>();
        cameraShake = GetComponent<CinemachineImpulseSource>();
        isDying = false;

        ResetMaterialProperties(); // Reinicia las propiedades del material
    }

    public void ReceiveDamage(int damage) // Recibe da�o (antes: "ReciveDamage")
    {
        if (!isInvulnerable)
        {
            StartCoroutine(InvulnerabilityCoroutine());
            cameraShake.GenerateImpulse();
            damageParticleEffect.Play();
            healthIndicators[Mathf.Max(0, currentHealth - 1)].GetComponent<Animator>().SetTrigger("Break");
            currentHealth -= damage;
            damageFeedback?.PlayFeedbacks();
            musicManager.Play_FX_PLayer_RecibirDano();

            if (currentHealth <= 0)
            {
                HandlePlayerDeath(); // Maneja la muerte del jugador (antes: "PlayerDeath")
            }
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;

        // Activa el color shifting, rim light y view shift
        playerMaterial.SetFloat(ColorShiftKey, 1);
        playerMaterial.SetFloat(RimLightKey, 1);
        playerMaterial.SetFloat(ViewShiftKey, 1);
        playerMaterial.SetFloat("_BaseColor_Step", 1f); // Aqu� se cambia el valor a 1 al recibir da�o

        // Esperamos antes de realizar la transici�n
        yield return new WaitForSeconds(invulnerabilityDuration);

        // Tiempo total para la transici�n de desactivaci�n
        float duration = invulnerabilityDuration;
        float elapsed = 0f;

        // Desactiva progresivamente el color shifting, rim light y view shift
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            playerMaterial.SetFloat(ColorShiftKey, 1 - t);
            playerMaterial.SetFloat(RimLightKey, 1 - t);
            playerMaterial.SetFloat(ViewShiftKey, 1 - t);
            float baseColorStepValue = Mathf.Lerp(1f, 0.5f, t); // Cambia de 1 a 0.5
            playerMaterial.SetFloat("_BaseColor_Step", baseColorStepValue);
            yield return null;
        }

        ResetMaterialProperties(); // Reinicia las propiedades del material
        isInvulnerable = false;
    }

    public void HandlePlayerDeath() // Maneja la muerte del jugador (antes: "PlayerDeath")
    {
        if (!isDying)
        {
            damageParticleEffect.Play();

            if (invulnerabilityCoroutine != null) // Detiene la coroutine si est� corriendo
            {
                StopCoroutine(invulnerabilityCoroutine);
                invulnerabilityCoroutine = null; // Reinicia la referencia
            }

            ResetMaterialProperties(); // Reinicia las propiedades del material

            isDying = true;
            StartCoroutine(DeathTransition()); // Inicia la transici�n de muerte
        }
    }

    private IEnumerator DeathTransition() // Transici�n de muerte (antes: "TransicionMuerte")
    {
        musicManager.Play_FX_Player_PolloMuerto();
        animator.SetTrigger("Die");
        gameManager.EndRound();
        EventSystem.current.SetSelectedGameObject(restartButtonFirstObject); // Selecciona el bot�n de reinicio
        Destroy(gameObject); // Destruye el objeto del jugador

        yield return new WaitForSeconds(2f);
    }

    public void IncreaseHealth(int extraLife) // Incrementa la vida del jugador (antes: "LifeUp")
    {
        currentHealth += extraLife;
        VFXManager.Instance.PlayEffect("HealthParticle", transform, Vector3.zero, Quaternion.Euler(-90f, 0f, 0f));

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        int index = Mathf.Clamp(currentHealth - 1, 0, healthIndicators.Length - 1);
        healthIndicators[index]?.GetComponent<Animator>()?.SetTrigger("UnBreak");
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    private void ResetMaterialProperties()
    {
        // Reinicia las propiedades visuales del material
        playerMaterial.SetFloat(ColorShiftKey, 0);
        playerMaterial.SetFloat(RimLightKey, 0);
        playerMaterial.SetFloat(ViewShiftKey, 0);
        playerMaterial.SetFloat("_BaseColor_Step", 0.5f);
    }
}