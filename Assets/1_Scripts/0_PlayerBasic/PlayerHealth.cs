using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] private int currentHealth = 3;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private Image healthBarImage; // AHORA una Image radial
    public GameObject restartButtonFirstObject;

    [Header("Damage System")]
    [SerializeField] private float invulnerabilityDuration = 1f;
    private bool isInvulnerable = false;
    [SerializeField] private ParticleSystem damageParticleEffect;
    public GameObject highScorePanel;
    [SerializeField] private Material playerMaterial;

    private static readonly int ColorShiftKey = Shader.PropertyToID("_Is_ColorShift");
    private static readonly int RimLightKey = Shader.PropertyToID("_RimLight");
    private static readonly int ViewShiftKey = Shader.PropertyToID("_Is_ViewShift");

    private Coroutine invulnerabilityCoroutine;
    private bool isDying = false;

    [Header("Animations and VFX")]
    private Animator animator;
    private CinemachineImpulseSource cameraShake;
    [SerializeField] private MusicManager musicManager;
    private GameManager gameManager;

    public MMFeedbacks damageFeedback;

    private void Start()
    {
        animator = GetComponent<Animator>();
        musicManager = FindAnyObjectByType<MusicManager>();
        gameManager = FindAnyObjectByType<GameManager>();
        cameraShake = GetComponent<CinemachineImpulseSource>();
        isDying = false;

        ResetMaterialProperties();
        UpdateHealthUI(); // Asegurarse que la barra empieza bien
    }

    public void ReceiveDamage(int damage)
    {
        if (!isInvulnerable)
        {
            StartCoroutine(InvulnerabilityCoroutine());
            cameraShake.GenerateImpulse();
            damageParticleEffect.Play();
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Proteger que no sea negativo
            damageFeedback?.PlayFeedbacks();
            musicManager.Play_FX_PLayer_RecibirDano();

            UpdateHealthUI(); // Actualizar barra de vida

            if (currentHealth <= 0)
            {
                HandlePlayerDeath();
            }
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;

        playerMaterial.SetFloat(ColorShiftKey, 1);
        playerMaterial.SetFloat(RimLightKey, 1);
        playerMaterial.SetFloat(ViewShiftKey, 1);
        playerMaterial.SetFloat("_BaseColor_Step", 1f);

        yield return new WaitForSeconds(invulnerabilityDuration);

        float duration = invulnerabilityDuration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            playerMaterial.SetFloat(ColorShiftKey, 1 - t);
            playerMaterial.SetFloat(RimLightKey, 1 - t);
            playerMaterial.SetFloat(ViewShiftKey, 1 - t);
            float baseColorStepValue = Mathf.Lerp(1f, 0.5f, t);
            playerMaterial.SetFloat("_BaseColor_Step", baseColorStepValue);

            yield return null;
        }

        ResetMaterialProperties();
        isInvulnerable = false;
    }

    public void HandlePlayerDeath()
    {
        if (!isDying)
        {
            damageParticleEffect.Play();

            if (invulnerabilityCoroutine != null)
            {
                StopCoroutine(invulnerabilityCoroutine);
                invulnerabilityCoroutine = null;
            }

            ResetMaterialProperties();
            isDying = true;
            StartCoroutine(DeathTransition());
        }
    }

    private IEnumerator DeathTransition()
    {
        musicManager.Play_FX_Player_PolloMuerto();
        animator.SetTrigger("Die");
        gameManager.EndRound();
        EventSystem.current.SetSelectedGameObject(restartButtonFirstObject);
        Destroy(gameObject);

        yield return new WaitForSeconds(2f);
    }

    public void IncreaseHealth(int extraLife)
    {
        currentHealth += extraLife;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Proteger
        VFXManager.Instance.PlayEffect("HealthParticle", transform, Vector3.zero, Quaternion.Euler(-90f, 0f, 0f));

        UpdateHealthUI(); // Actualizar barra
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    private void UpdateHealthUI()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = (float) currentHealth / maxHealth;
        }
    }

    private void ResetMaterialProperties()
    {
        playerMaterial.SetFloat(ColorShiftKey, 0);
        playerMaterial.SetFloat(RimLightKey, 0);
        playerMaterial.SetFloat(ViewShiftKey, 0);
        playerMaterial.SetFloat("_BaseColor_Step", 0.5f);
    }
}
