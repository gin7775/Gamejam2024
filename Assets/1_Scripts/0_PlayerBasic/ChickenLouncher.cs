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
    [SerializeField] private int pickUpRange = 5; // Rango de recogida de pollos
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
    private bool isInRange = false;
    private Animator anim; // Controlador de animaciones

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        boxCollider = GetComponent<BoxCollider>();
    }
    private void Update()
    {
        DetectNearbyChickens();
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
        // Si ya tienes un pollo, lo reemplazamos
        if (currentChickenType > 0 && isInRange && selectedChicken != null)
        {
            musicManager.Play_FX_RecogerPollo();  // Sonido de recoger el pollo

            // Destruimos el pollo que el jugador tiene en la mano
            Destroy(currentWeapon);
            currentChickenType = 0;  // Restablecemos el pollo en la mano

            // Llamamos a la función para reemplazarlo con el pollo que está en el suelo
            ReplaceChickenWithNew(selectedChicken);

            // Destruimos el pollo en el suelo
            Destroy(selectedChicken);

            // Desmarcamos el estado de estar en rango
            isInRange = false;
            selectedChicken = null;
        }
    }

    private void DetectNearbyChickens()
    {

        Vector3 detectionCenter = transform.position + Vector3.up * 1f; // Ajustar 1f dependiendo de la altura del jugador

        // Verificamos si el jugador ya tiene un pollo en la mano
        if (currentChickenType > 0)  // O puedes usar selectedChicken != null, dependiendo de cómo lo gestiones
        {
            // Si ya tienes un pollo, no hacemos nada, salimos de la función
            return;
        }

        // Usamos OverlapSphere para detectar todos los objetos dentro del rango desde la posición ajustada
        Collider[] hitColliders = Physics.OverlapSphere(detectionCenter, pickUpRange);

        bool chickenDetected = false; // Bandera para saber si hemos detectado un pollo

        foreach (var hitCollider in hitColliders)
        {
            // Verificamos si el collider tiene el tag "CorpseCollider"
            if (hitCollider.CompareTag("CorpseCollider"))
            {
                // Si encontramos un pollo, lo seleccionamos como el pollo más cercano
                selectedChicken = hitCollider.gameObject;
                isInRange = true; // Marcamos que hay un pollo en rango
                chickenDetected = true; // Hemos detectado un pollo

                // Llamamos al método HandleCorpseCollision para gestionar la recogida del pollo
                HandleCorpseCollision(hitCollider);

                break; // Salimos después de encontrar el primer pollo
            }
        }

        // Si no encontramos ningún pollo, desmarcamos el estado
        if (!chickenDetected)
        {
            isInRange = false;
            selectedChicken = null;
        }
    }
    void ReplaceChickenWithNew(GameObject newChicken)
    {
        // Destruimos el pollo actual que el jugador tiene en la mano (si existe)
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        currentChickenType = 0; // Restablecemos el tipo de pollo en la mano

        // Obtenemos el tipo de pollo del nuevo pollo que se ha recogido
        int chickenType = newChicken.GetComponent<ChickenCorpse>().chickenType;

        // Actualizamos el tipo de pollo en la mano
        RetrieveChicken(chickenType);

        // Creamos el nuevo pollo en la mano del jugador
        GameObject weaponChicken = ragdolls[chickenType - 1];  // Usamos el prefab correspondiente al tipo de pollo
        currentWeapon = Instantiate(weaponChicken, handPosition.position, Quaternion.Euler(transform.rotation.eulerAngles), transform);

        // Destruimos el pollo en el suelo después de que lo hayamos recogido
        Destroy(newChicken);  // Este es el pollo del suelo
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
    private Dictionary<int, string> chickenVFXEffects = new Dictionary<int, string>
{
    { 1, "WhiteMuzzle" },  // Pollo normal
    { 2, "RedMuzzle" },   // Pollo grande
    { 3, "RedMuzzle" },   //Pollo Bomba
    { 4, "RedMuzzle" }, // Pollo Rápido
    { 5, "RedMuzzle" },   // Pollo Disparo
    { 6, "RedMuzzle" }  // Pollo curativo
};


    void Shoot(int ammoType)
    {
        enableBoxCollider = false;
    Vector3 projectilePos = CalculateProjectileStartPosition();

    if (currentChickenType == 6) // Pollo que da vida extra
        playerHealth.IncreaseHealth(1);

    if (!gameModeFrenezzi)
    {
            int chickenTypeBeforeLaunch = currentChickenType;  // Guardamos el tipo antes de lanzarlo

            HandleProjectileLaunch(ammoType, projectilePos);

            // Verifica antes de llamar el efecto
            Debug.Log($"Intentando reproducir efecto para tipo: {chickenTypeBeforeLaunch}");

            if (chickenVFXEffects.TryGetValue(chickenTypeBeforeLaunch, out string effectName)) // Busca si el tipo de pollo tiene un efecto
            {
                VFXManager.Instance.PlayEffect(effectName, transform, new Vector3(0.09000015f, 0.8f, 0.2800007f), Quaternion.Euler(0f, 0f, 0f));
            }
            else
            {
                Debug.LogWarning($"No hay efecto asociado para el tipo de pollo {chickenTypeBeforeLaunch}");
            }

            currentChickenType = 0;  // Ahora lo reseteamos después de todo.
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

    //private void OnTriggerEnter(Collider other)
    //{
    //    if ((enableBoxCollider || currentChickenType <= 0) && other.gameObject.CompareTag("CorpseCollider"))
    //    {
    //        HandleCorpseCollision(other);
    //    }
    //}

    void HandleCorpseCollision(Collider other)
    {
        // Comprobamos que el objeto que hemos detectado es un pollo
        if (other.gameObject.GetComponentInParent<ChickenCorpse>() != null)
        {
            musicManager.Play_FX_RecogerPollo();
            int type = other.gameObject.GetComponentInParent<ChickenCorpse>().chickenType;

            // Crear el nuevo pollo en la mano
            GameObject weaponChicken = ragdolls[type - 1];
            weaponChicken.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            // Destruimos el pollo en el suelo (si existe)
            if (selectedChicken != null)
            {
                Destroy(selectedChicken);
            }

            currentWeapon = Instantiate(weaponChicken, handPosition.position, Quaternion.Euler(transform.rotation.eulerAngles), transform);

            RetrieveChicken(type); // Actualizar el tipo de pollo en la mano

            if (currentProjectile.Count == 0)
                currentProjectile.Add(currentWeapon);
            else
                currentProjectile[0] = currentWeapon;

            enableBoxCollider = false;
        }
    }
}

