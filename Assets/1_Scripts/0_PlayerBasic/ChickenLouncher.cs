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
    // Meter nuevo script gestor de vidas
    //[SerializeField] int health = 3;
    //[SerializeField] int maxHealth = 3;
    //[SerializeField] GameObject[] eggs;
    [SerializeField] Collider[] shickensDetected;
    [SerializeField] int pickUpRange = 3;
    [SerializeField] float distanciaComparativa,distanciaActual;
    [SerializeField] GameObject polloElegido;
    [SerializeField] MusicManager musicManager;
    [SerializeField] Transform handPosition;
    public List <GameObject> currentProyectile;
    public GameObject[] shildCounter;
    GameObject proyectile;
    Vector3 projectilePos;
    private Animator anim;

    //Esto es para la muerte
    //bool muriendo;
    [SerializeField] private bool gameModeFrenezzi = false;

    /*
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
    CinemachineImpulseSource cinemachineImpulseSource;*/

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
        //cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        //muriendo = false;
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

        //Debug.Log("Dispara");
    }

    public void OnAttack(InputValue value)
    {
        Debug.Log("ATTACK ANIMATION");
        Attack(chickenType);
        //Debug.Log("Ataca");
    }

    public void OnPick(InputValue value)
    {
        Debug.Log("RECOGER");
        boxCollider.enabled = true;
        enableBoxCollider = true;
        //RetrieveChicken(chickenType);
        //Debug.Log("Coge");

        StartCoroutine(DisableColliderAfterTime(1.5f));
    }


    void RetrieveChicken(int chickenNumber)
    {
        anim.SetBool("Carrying", true);
        chickenType = chickenNumber;
        if (chickenType == 3)
        {
            //LifeUp(1);
            playerHealth.LifeUp(1);
        }
        if (chickenType >= 5 || chickenType < 0)
        {
            chickenType = 1;
        }
    }

    // Corutina para desactivar el BoxCollider después de un tiempo específico
    IEnumerator DisableColliderAfterTime(float delay)
    {
        // Espera el tiempo especificado (1.5 segundos)
        yield return new WaitForSeconds(delay);

        // Desactiva el BoxCollider
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
        projectilePos = transform.position;
        projectilePos += transform.forward;
        projectilePos += transform.up;

        if (!gameModeFrenezzi)
        {
            switch (AmmoType)
            {
                case 0:
                    //Debug.Log("Got no chickens");
                    break;

                case 1:
                    //Debug.Log("Lounching Chicken");
                    proyectile = Instantiate(proyectiles[0], projectilePos, Quaternion.identity);
                    proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);
                    musicManager.Play_FX_PLayer_DispararPollo();                                            //Audio

                    chickenType = 0;
                    break;

                case 2:
                    projectilePos = transform.position;
                    projectilePos += transform.forward;
                    projectilePos += transform.up;
                    //Debug.Log("Lounching Chicken");
                    proyectile = Instantiate(proyectiles[1], projectilePos, Quaternion.identity);
                    proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);
                    musicManager.Play_FX_PLayer_DispararPollo();                                            //Audio


                    chickenType = 0;
                    break;

                case 3:
                    projectilePos = transform.position;
                    projectilePos += transform.forward;
                    projectilePos += transform.up;
                    //Debug.Log("Lounching Chicken");
                    proyectile = Instantiate(proyectiles[2], projectilePos, Quaternion.identity);
                    proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);
                    musicManager.Play_FX_PLayer_DispararPollo();                                            //Audio


                    chickenType = 0;
                    break;

                case 4:
                    projectilePos = transform.position;
                    projectilePos += transform.forward;
                    projectilePos += transform.up;
                    //Debug.Log("Lounching Chicken");
                    proyectile = Instantiate(proyectiles[3], projectilePos, Quaternion.identity);
                    proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);
                    musicManager.Play_FX_PLayer_DispararPollo();                                            //Audio

                    chickenType = 0;
                    break;
            }
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
        else
        {
            if (currentProyectile != null)
            {
                foreach (GameObject pollo in currentProyectile)
                {
                    if (pollo.GetComponent<ChickenCorpse>() != null)
                    {
                        pollo.transform.SetParent(handPosition, false);
                        RetrieveChicken(pollo.GetComponent<ChickenCorpse>().chickenType);
                        switch (pollo.GetComponent<ChickenCorpse>().chickenType)
                        {
                            case 0:
                                //Debug.Log("Got no chickens");
                                break;

                            case 1:
                                //Debug.Log("Lounching Chicken");
                                proyectile = Instantiate(proyectiles[0], projectilePos, Quaternion.identity);
                                proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);

                                //chickenType = 0;
                                break;

                            case 2:
                                projectilePos = transform.position;
                                projectilePos += transform.forward;
                                projectilePos += transform.up;
                                //Debug.Log("Lounching Chicken");
                                proyectile = Instantiate(proyectiles[1], projectilePos, Quaternion.identity);
                                proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);

                                //chickenType = 0;
                                break;

                            case 3:
                                projectilePos = transform.position;
                                projectilePos += transform.forward;
                                projectilePos += transform.up;
                                //Debug.Log("Lounching Chicken");
                                proyectile = Instantiate(proyectiles[2], projectilePos, Quaternion.identity);
                                proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);

                                //chickenType = 0;
                                break;

                            case 4:
                                projectilePos = transform.position;
                                projectilePos += transform.forward;
                                projectilePos += transform.up;
                                //Debug.Log("Lounching Chicken");
                                proyectile = Instantiate(proyectiles[3], projectilePos, Quaternion.identity);
                                proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);

                                //chickenType = 0;
                                break;
                        }
                        //currentProyectile.Remove(pollo);
                        Destroy(pollo);
                    }
                }
                chickenType = 0;
                currentProyectile.Clear();
            }
        }
    }

    public void Attack(int AmmoType)
    {
       
            switch (AmmoType)
            {
                case 0:
                    if (Time.time >= nextAttackTime) //Inicio Comprobación Cooldown Ataque
                    {
                        /*CanAttack = true;
                    }
                    if (CanAttack)
                    {
                        CanAttack = false;*/
                        anim.SetTrigger("Attack");
                        nextAttackTime = Time.time + AttackCooldown; //Final Comprobación Cooldown Ataque
                        HeadBut();
                    } 

                    //Debug.Log("Got no chickens");
                    //HeadBut();
                    break;
                case 1:
                    ChickenSwing();
                    UpdateWeapon();
                    break;
                case 2:
                    ChickenSwing();
                    UpdateWeapon();
                    break;
                case 3:
                    ChickenSwing();
                    UpdateWeapon();
                    break;
                case 4:
                    ChickenSwing();
                    UpdateWeapon();
                    break;
            } 
        
    }

    public void HeadBut()
    {
        if (headBox != null)
        {
            //Debug.Log("Headbutting");
            StartCoroutine(ActivateColliderChicken(headBox));
        }
        else
        {
            //Debug.Log("Headbut Collider is missing");
        }
    }

    public void ChickenSwing()
    {
        if (swingBox != null)
        {
            //Debug.Log("Swinging a chicken");
            StartCoroutine(ActivateCollider(swingBox));
        }
        else
        {
            //Debug.Log("Swing Collider is missing");
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
        yield return new WaitForSeconds(0.2f);
        collider.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si el trigger que activó el evento es el CapsuleCollider
        if (other == capsuleCollider)
        {
            Debug.Log("El CapsuleCollider ha activado el trigger.");
            // Lógica específica para el CapsuleCollider
        }

        // Si el trigger que activó el evento es el BoxCollider
        if (other == boxCollider)
        {
            Debug.Log("El BoxCollider ha activado el trigger.");

            if (other.gameObject.CompareTag("Corpse"))
            {
                Debug.Log("Detecta Corpse 1");
            }
            // Lógica específica para el BoxCollider
        }

        if (enableBoxCollider)
        {
            if (other.gameObject.CompareTag("CorpseCollider"))
            {
                Debug.Log("Detecta Corpse 2");

                if (other.gameObject.GetComponentInParent<ChickenCorpse>() != null)
                {
                    musicManager.Play_FX_RecogerPollo();
                    int type = other.gameObject.GetComponentInParent<ChickenCorpse>().chickenType;
                    Destroy(other.GetComponentInParent<ChickenCorpse>().gameObject);
                    Destroy(arma);
                    Debug.Log("type:" + type);
                    Debug.Log(ragdolls[type - 1]);

                    arma = Instantiate(ragdolls[type - 1], handPosition.position, Quaternion.identity, GetComponent<ChickenLouncher>().gameObject.transform);
                    RetrieveChicken(type);

                    if (currentProyectile.Count == 0)
                        currentProyectile.Add(arma);
                    else
                        currentProyectile[0] = arma;
                }

                enableBoxCollider = false;
            }
        }

        // Debug.Log("He colisionado con " + other.gameObject.name);
        if (other.gameObject.CompareTag("Corpse"))
        {
            Debug.Log("Detecta Corpse 3");
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
