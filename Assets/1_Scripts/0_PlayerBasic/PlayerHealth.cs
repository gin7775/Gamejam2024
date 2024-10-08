using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Gestor de Vidas Player
    [SerializeField] int health = 3;
    [SerializeField] int maxHealth = 3;
    [SerializeField] GameObject[] eggs;

    [Header("RECIBIR DAÑO")]
    [SerializeField] private float invulnerabilityDuration = 1f;
    private bool isInvulnerable = false;
    [SerializeField] private ParticleSystem hitParticle;
    public GameObject RetryButton;
    [SerializeField] private Material playerMaterial;
    private static readonly int IsColorShift = Shader.PropertyToID("_Is_ColorShift");
    private static readonly int IsRimLight = Shader.PropertyToID("_Is_RimLight");
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
    }

    public void ReciveDamage(int damage)
    {
        if (!isInvulnerable)
        {


            StartCoroutine(InvulnerabilityCoroutine());
            cinemachineImpulseSource.GenerateImpulse();
            hitParticle.Play();
            UpdateLifeUI();
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

        // Tiempo total para la transición de desactivación
        float duration = 1f;
        float elapsed = 0f;


        yield return new WaitForSeconds(1f);

        // Desactiva progresivamente el color shifting, rim light y view shift
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;


            float t = Mathf.Clamp01(elapsed / duration);


            playerMaterial.SetFloat(IsColorShift, 1 - t);
            playerMaterial.SetFloat(IsRimLight, 1 - t);
            playerMaterial.SetFloat(IsViewShift, 1 - t);

            yield return null;
        }


        playerMaterial.SetFloat(IsColorShift, 0);
        playerMaterial.SetFloat(IsRimLight, 0);
        playerMaterial.SetFloat(IsViewShift, 0);

        isInvulnerable = false;
    }

    private void UpdateLifeUI()
    {
        if (eggs != null)
        {
            eggs[health - 1].GetComponent<Animator>().SetTrigger("Break");
        }
    }

    public void PlayerDeath()
    {
        if (!muriendo)
        {
            hitParticle.Play();
            if (invulnerabilityCoroutine != null) // Detén la coroutine si está corriendo
            {
                StopCoroutine(invulnerabilityCoroutine);
                invulnerabilityCoroutine = null; // Reinicia la referencia
            }

            // Desactiva efectos de invulnerabilidad
            playerMaterial.SetFloat(IsColorShift, 0);
            playerMaterial.SetFloat(IsRimLight, 0);
            playerMaterial.SetFloat(IsViewShift, 0);
            muriendo = true;
            StartCoroutine(TransicionMuerte());
        }
    }

    IEnumerator TransicionMuerte()
    {
        musicManager.Play_FX_Player_PolloMuerto();
        anim.SetTrigger("Die");
        RetryButton.gameObject.SetActive(true);
        Destroy(this.gameObject);

        yield return new WaitForSeconds(2f);

        //Debug.Log("Ye dead!");
        //RetryButton.gameObject.SetActive(true);
        //Destroy(this.gameObject);
    }

    public void LifeUp(int extraLife)
    {
        health += extraLife;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

}
