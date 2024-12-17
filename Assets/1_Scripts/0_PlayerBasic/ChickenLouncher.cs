using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChickenLouncher : MonoBehaviour
{
    [Header("Chicken Settings")]
    public int currentChickenType = 0; // Tipo de pollo seleccionado
    [SerializeField] private GameObject[] proyectiles; // Prefabs de proyectiles de pollo
    [SerializeField] private GameObject[] ragdolls; // Prefabs de ragdolls de pollo
    [SerializeField] private float proyectileForce = 10; // Fuerza de los proyectiles
    public int bigChickenImpulseForce = 10; // Impulso especial para el pollo grande

    [Header("Chicken Usage")]
    [SerializeField] private int chickenCurrentUses = 0; // Uso actual del pollo
    [SerializeField] private int chickenMaxUses = 3; // Usos máximos permitidos
    [SerializeField] private Transform handPosition; // Posición donde aparece el pollo
    public List<GameObject> currentProjectile; // Proyectiles activos actualmente
    private GameObject currentWeapon; // Referencia al arma actual

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown = 0.5f; // Tiempo entre ataques
    [SerializeField] private float nextAllowedAttackTime = 0f; // Tiempo permitido para el siguiente ataque
    [SerializeField] private GameObject headBox; // Collider para ataques con la cabeza
    [SerializeField] private GameObject swingBox; // Collider para ataques cuerpo a cuerpo

    [Header("Player Health")]
    [SerializeField] private PlayerHealth playerHealth; // Referencia al script de salud del jugador

    [Header("Detection Settings")]
    [SerializeField] private Collider[] chickensDetected; // Pollos detectados en el rango
    [SerializeField] private int pickUpRange = 3; // Rango de recogida de pollos
    [SerializeField] private float comparativeDistance, currentDistance; // Distancias para la lógica de recogida
    [SerializeField] private GameObject selectedChicken; // Pollo seleccionado más cercano

    [Header("Music Manager")]
    [SerializeField] private MusicManager musicManager; // Gestor de música y sonidos

    [Header("Game Mode Settings")]
    [SerializeField] private bool gameModeFrenezzi = false; // Modo de juego Frenezzi

    [Header("Colliders")]
    private CapsuleCollider capsuleCollider;
    private BoxCollider boxCollider;
    private bool enableBoxCollider = false;

    private Animator anim; // Controlador de animaciones

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        musicManager = FindAnyObjectByType<MusicManager>();
        comparativeDistance = 1000;
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    public void OnShoot(InputValue value)
    {
        anim.SetBool("Carrying", false);
        anim.SetTrigger("Throw");
        musicManager.Play_FX_RecogerPollo();
        Shoot(currentChickenType);
    }

    public void OnAttack(InputValue value)
    {
        enableBoxCollider = false;

        if (Time.time >= nextAllowedAttackTime)
        {
            nextAllowedAttackTime = Time.time + attackCooldown;
            anim.SetTrigger("Attack");
            PerformAttack(currentChickenType);
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
        currentChickenType = chickenNumber;

        if (currentChickenType >= 10 || currentChickenType < 0)
            currentChickenType = 1;
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
            currentChickenType = 0;
    }

    void Shoot(int ammoType)
    {
        enableBoxCollider = false;
        Vector3 projectilePos = CalculateProjectileStartPosition();

        if (currentChickenType == 6) // Pollo que da vida extra
            playerHealth.IncreaseHealth(1);

        if (!gameModeFrenezzi)
        {
            HandleProjectileLaunch(ammoType, projectilePos);
            ClearProjectiles();
        }
        else
        {
            HandleFrenezziMode();
        }
    }

    Vector3 CalculateProjectileStartPosition()
    {
        if (currentChickenType == 2)
        {
            return transform.position + (transform.forward * 2) + (transform.up * 2);
        }
        else
        {
            return transform.position + transform.forward + transform.up;
        }
    }

    void HandleProjectileLaunch(int ammoType, Vector3 projectilePos)
    {
        if (ammoType > 0 && ammoType <= proyectiles.Length)
        {
            GameObject projectile = Instantiate(proyectiles[ammoType - 1], projectilePos, Quaternion.identity);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (currentChickenType == 2)
            {
                rb.AddForce(transform.forward * bigChickenImpulseForce, ForceMode.Impulse);
            }
            else if (currentChickenType == 4)
            {
                rb.AddForce(transform.forward * proyectileForce * 1.5f);
            }
            else
            {
                rb.AddForce(transform.forward * proyectileForce);
            }

            musicManager.Play_FX_PLayer_DispararPollo();
            currentChickenType = 0;
        }
    }

    void ClearProjectiles()
    {
        if (currentProjectile != null)
        {
            foreach (GameObject chicken in currentProjectile)
            {
                chicken.transform.SetParent(handPosition, false);
                Destroy(chicken);
            }
            currentProjectile.Clear();
        }
    }

    void HandleFrenezziMode()
    {
        if (currentProjectile != null)
        {
            foreach (GameObject chicken in currentProjectile)
            {
                if (chicken.GetComponent<ChickenCorpse>() != null)
                {
                    chicken.transform.SetParent(handPosition, false);
                    RetrieveChicken(chicken.GetComponent<ChickenCorpse>().chickenType);
                    HandleProjectileLaunch(chicken.GetComponent<ChickenCorpse>().chickenType, CalculateProjectileStartPosition());
                    Destroy(chicken);
                }
            }
            currentChickenType = 0;
            currentProjectile.Clear();
        }
    }

    public void PerformAttack(int ammoType)
    {
        if (ammoType == 0)
        {
            HeadBut();
        }
        else if (ammoType >= 1 && ammoType <= 4)
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
        if ((enableBoxCollider || currentChickenType <= 0) && other.gameObject.CompareTag("CorpseCollider"))
        {
            HandleCorpseCollision(other);
        }
    }

    void HandleCorpseCollision(Collider other)
    {
        if (other.gameObject.GetComponentInParent<ChickenCorpse>() != null)
        {
            musicManager.Play_FX_RecogerPollo();
            int type = other.gameObject.GetComponentInParent<ChickenCorpse>().chickenType;
            Destroy(other.GetComponentInParent<ChickenCorpse>().gameObject);
            Destroy(currentWeapon);
            GameObject weaponChicken = ragdolls[type - 1];
            weaponChicken.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            currentWeapon = Instantiate(weaponChicken, handPosition.position, Quaternion.Euler(transform.rotation.eulerAngles), transform);

            RetrieveChicken(type);

            if (currentProjectile.Count == 0)
                currentProjectile.Add(currentWeapon);
            else
                currentProjectile[0] = currentWeapon;

            enableBoxCollider = false;
        }
    }
}

