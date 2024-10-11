using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class ChickenLouncher : MonoBehaviour
{
    public int chickenType = 0;
    [SerializeField] GameObject[] proyectiles = null;
    [SerializeField] GameObject[] ragdolls = null;
    [SerializeField] float proyectileForce = 10;
    [SerializeField] int chickenCurrentUses = 0;
    [SerializeField] int chickenMaxUses = 3;
    [SerializeField] GameObject headBox;
    [SerializeField] GameObject swingBox;

    // Control del Player
    [SerializeField] float AttackCooldown = 0.5f;
    [SerializeField] float nextAttackTime = 0f;

    // Gestor de Vidas Player
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] Collider[] shickensDetected;
    [SerializeField] int pickUpRange = 3;
    [SerializeField] float distanciaComparativa, distanciaActual;
    [SerializeField] GameObject polloElegido;
    [SerializeField] MusicManager musicManager;
    [SerializeField] Transform handPosition;
    public List<GameObject> currentProyectile;
    public GameObject[] shildCounter;
    GameObject proyectile;
    Vector3 projectilePos;
    private Animator anim;

    [SerializeField] private bool gameModeFrenezzi = false;

    private CapsuleCollider capsuleCollider;
    private BoxCollider boxCollider;
    private bool enableBoxCollider = false;
    private GameObject arma;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        musicManager = FindAnyObjectByType<MusicManager>();
        distanciaComparativa = 1000;
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    public void OnShoot(InputValue value)
    {
        anim.SetBool("Carrying", false);
        anim.SetTrigger("Throw");
        musicManager.Play_FX_RecogerPollo();
        Shoot(chickenType);
    }

    public void OnAttack(InputValue value)
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + AttackCooldown;
            anim.SetTrigger("Attack");
            PerformAttack(chickenType);
        }
    }

    public void OnPick(InputValue value)
    {
        boxCollider.enabled = true;
        enableBoxCollider = true;
        StartCoroutine(DisableColliderAfterTime(1.5f));
    }

    void RetrieveChicken(int chickenNumber)
    {
        anim.SetBool("Carrying", true);
        chickenType = chickenNumber;
        if (chickenType == 3)
        {
            playerHealth.LifeUp(1);
        }
        if (chickenType >= 5 || chickenType < 0)
        {
            chickenType = 1;
        }
    }

    IEnumerator DisableColliderAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        boxCollider.enabled = false;
        enableBoxCollider = false;
    }

    void UpdateWeapon()
    {
        chickenCurrentUses++;
        if (chickenCurrentUses == chickenMaxUses)
        {
            chickenType = 0;
        }
    }

    void Shoot(int AmmoType)
    {
        projectilePos = CalculateProjectileStartPosition();
        if (!gameModeFrenezzi)
        {
            HandleProjectileLaunch(AmmoType);
            ClearProjectiles();
        }
        else
        {
            HandleFrenezziMode();
        }
    }

    Vector3 CalculateProjectileStartPosition()
    {
        return transform.position + transform.forward + transform.up;
    }

    void HandleProjectileLaunch(int AmmoType)
    {
        if (AmmoType > 0 && AmmoType <= proyectiles.Length)
        {
            proyectile = Instantiate(proyectiles[AmmoType - 1], projectilePos, Quaternion.identity);
            proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);
            musicManager.Play_FX_PLayer_DispararPollo();
            chickenType = 0;
        }
    }

    void ClearProjectiles()
    {
        if (currentProyectile != null)
        {
            foreach (GameObject pollo in currentProyectile)
            {
                pollo.transform.SetParent(handPosition, false);
                Destroy(pollo);
            }
            currentProyectile.Clear();
        }
    }

    void HandleFrenezziMode()
    {
        if (currentProyectile != null)
        {
            foreach (GameObject pollo in currentProyectile)
            {
                if (pollo.GetComponent<ChickenCorpse>() != null)
                {
                    pollo.transform.SetParent(handPosition, false);
                    RetrieveChicken(pollo.GetComponent<ChickenCorpse>().chickenType);
                    HandleProjectileLaunch(pollo.GetComponent<ChickenCorpse>().chickenType);
                    Destroy(pollo);
                }
            }
            chickenType = 0;
            currentProyectile.Clear();
        }
    }

    public void PerformAttack(int AmmoType)
    {
        if (AmmoType == 0)
        {
            HeadBut();
        }
        else if (AmmoType >= 1 && AmmoType <= 4)
        {
            ChickenSwing();
            UpdateWeapon();
        }
    }

    public void HeadBut()
    {
        if (headBox != null)
        {
            StartCoroutine(ActivateColliderChicken(headBox));
        }
    }

    public void ChickenSwing()
    {
        if (swingBox != null)
        {
            StartCoroutine(ActivateCollider(swingBox));
        }
    }

    IEnumerator ActivateCollider(GameObject collider)
    {
        yield return new WaitForSeconds(0.2f);
        collider.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        collider.SetActive(false);
    }

    IEnumerator ActivateColliderChicken(GameObject collider)
    {
        yield return new WaitForSeconds(0.15f);
        collider.SetActive(true);
        enableBoxCollider = true;
        yield return new WaitForSeconds(0.2f);
        collider.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enableBoxCollider && other.gameObject.CompareTag("CorpseCollider"))
        {
            HandleCorpseCollision(other);
            enableBoxCollider = false;
        }
    }

    void HandleCorpseCollision(Collider other)
    {
        if (other.gameObject.GetComponentInParent<ChickenCorpse>() != null)
        {
            musicManager.Play_FX_RecogerPollo();
            int type = other.gameObject.GetComponentInParent<ChickenCorpse>().chickenType;
            Destroy(other.GetComponentInParent<ChickenCorpse>().gameObject);
            Destroy(arma);
            GameObject weaponChicken = ragdolls[type - 1];
            GameObject player = GetComponent<ChickenLouncher>().gameObject;

            // Inicializamos la position y la rotacion a 0
            weaponChicken.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

            arma = Instantiate(weaponChicken, handPosition.position, Quaternion.Euler(player.transform.rotation.eulerAngles), player.gameObject.transform);
            RetrieveChicken(type);

            if (currentProyectile.Count == 0)
                currentProyectile.Add(arma);
            else
                currentProyectile[0] = arma;
        }
    }

    // GESTION DE VIDA ANTIGUA

    // BORRAR DE AQUÍ
    // SOLO CUANDO SE HAYA COMPROBADO QUE TODO FUNCIONA BIEN
    /*
    void LifeUp(int extraLife)
    {
        health += extraLife;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
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

    void PlayerDeath()
    {
        if(!muriendo) 
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
    */
    // HASTA AQUÍ

}
