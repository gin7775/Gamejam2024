using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerHealth : MonoBehaviour
{
    // Gestor de Vidas Player
    [SerializeField] int health = 3;
    [SerializeField] int maxHealth = 3;
    [SerializeField] GameObject[] eggs;
    public GameObject firstGameObjectRetry;
    [Header("RECIBIR DAÑO")]
    [SerializeField] private float invulnerabilityDuration = 1f;
    private bool isInvulnerable = false;
    [SerializeField] private ParticleSystem hitParticle;
    public GameObject RetryButton;
    [SerializeField] private Material playerMaterial;
    private static readonly int IsColorShift = Shader.PropertyToID("_Is_ColorShift");
    private static readonly int IsRimLight = Shader.PropertyToID("_RimLight");
    private static readonly int IsViewShift = Shader.PropertyToID("_Is_ViewShift");
    private Coroutine invulnerabilityCoroutine;
    private bool muriendo = false;
    
    // Animations and VFX
    private Animator anim;
    CinemachineImpulseSource cinemachineImpulseSource;
    [SerializeField] MusicManager musicManager;

    void Start()
    {
        anim = GetComponent<Animator>();
        musicManager = FindAnyObjectByType<MusicManager>();
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        muriendo = false;
        playerMaterial.SetFloat(IsColorShift, 0);
        playerMaterial.SetFloat(IsRimLight, 0);
        playerMaterial.SetFloat(IsViewShift, 0);
        playerMaterial.SetFloat("_BaseColor_Step", 0.5f);
    }

    public void ReciveDamage(int damage)
    {
        if (!isInvulnerable)
        {
            StartCoroutine(InvulnerabilityCoroutine());
            cinemachineImpulseSource.GenerateImpulse();
            hitParticle.Play();
            eggs[Mathf.Max(0, health - 1)].GetComponent<Animator>().SetTrigger("Break");
            health -= damage;

            musicManager.Play_FX_PLayer_RecibirDaño();

            if (health <= 0)
            {
                PlayerDeath();
            }
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;

        // Activa el color shifting, rim light y view shift
        playerMaterial.SetFloat(IsColorShift, 1);
        playerMaterial.SetFloat(IsRimLight, 1);


        playerMaterial.SetFloat(IsViewShift, 1);

        // Aquí se cambia el valor a 1 al recibir daño
        playerMaterial.SetFloat("_BaseColor_Step", 1f);

        // Esperamos antes de realizar la transición
        yield return new WaitForSeconds(invulnerabilityDuration);
        
        // Tiempo total para la transición de desactivación
        float duration = invulnerabilityDuration;
        float elapsed = 0f;

        // Desactiva progresivamente el color shifting, rim light y view shift
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            playerMaterial.SetFloat(IsColorShift, 1 - t);
            playerMaterial.SetFloat(IsRimLight, 1 - t);
            playerMaterial.SetFloat(IsViewShift, 1 - t);
            float baseColorStepValue = Mathf.Lerp(1f, 0.5f, t); // Cambia de 1 a 0.5
            playerMaterial.SetFloat("_BaseColor_Step", baseColorStepValue);
            yield return null;
        }

        playerMaterial.SetFloat(IsColorShift, 0);
        playerMaterial.SetFloat(IsRimLight, 0);
        playerMaterial.SetFloat(IsViewShift, 0);
        playerMaterial.SetFloat("_BaseColor_Step", 0.5f);

        isInvulnerable = false;

    }

    public void PlayerDeath()
    {
        if (!muriendo)
        {
            hitParticle.Play();
            if (invulnerabilityCoroutine != null) // Detiene la coroutine si está corriendo
            {
                StopCoroutine(invulnerabilityCoroutine);
                invulnerabilityCoroutine = null; // Reinicia la referencia
            }

            // Desactiva efectos de invulnerabilidad
            playerMaterial.SetFloat(IsColorShift, 0);
            playerMaterial.SetFloat(IsRimLight, 0);
            playerMaterial.SetFloat(IsViewShift, 0);
            playerMaterial.SetFloat("_BaseColor_Step", 0.5f);

            muriendo = true;
            StartCoroutine(TransicionMuerte());
        }
    }

    IEnumerator TransicionMuerte()
    {
        musicManager.Play_FX_Player_PolloMuerto();
        anim.SetTrigger("Die");
        RetryButton.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstGameObjectRetry);
        Destroy(this.gameObject);

        yield return new WaitForSeconds(2f);
    }

    public void LifeUp(int extraLife)
    {
        health += extraLife;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        eggs[Mathf.Max(0, health - 1)].GetComponent<Animator>().SetTrigger("UnBreak");
    }
}
