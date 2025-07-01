using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChickenLouncher : MonoBehaviour
{
    [Header("Chicken Settings")]
    public int currentChickenType = 0; // Tipo de pollo seleccionado
    public GameObject[] proyectiles; // Prefabs de proyectiles de pollo
    [SerializeField] private GameObject[] ragdolls; // Prefabs de ragdolls de pollo
    [SerializeField] private float proyectileForce = 10; // Fuerza de los proyectiles
    public int bigChickenImpulseForce = 10; // Impulso especial para el pollo grande

    [Header("Chicken Usage")]
    //[SerializeField] private int chickenCurrentUses = 0; // Uso actual del pollo
    //[SerializeField] private int chickenMaxUses = 3; // Usos maximos permitidos
    [SerializeField] private Transform handPosition; // Posici�n donde aparece el pollo
    public List<GameObject> currentProjectile; // Proyectiles activos actualmente
    private GameObject currentWeapon; // Referencia al arma actual

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown = 0.5f; // Tiempo entre ataques
    [SerializeField] private float nextAllowedAttackTime = 0f; // Tiempo permitido para el siguiente ataque
    [SerializeField] private GameObject headBox; // Collider para ataques con la cabeza
    [SerializeField] private GameObject swingBox; // Collider para ataques cuerpo a cuerpo

    [Header("Autoshoot Mode")]
    public bool autoshoot = false;
    public float cadence = 1f;

    [Header("Player Health")]
    [SerializeField] private PlayerHealth playerHealth; // Referencia al script de salud del jugador

    [Header("Detection Settings")]
    [SerializeField] private Collider[] chickensDetected; // Pollos detectados en el rango
    [SerializeField] private int pickUpRange = 5; // Rango de recogida de pollos
    [SerializeField] private float currentDistance; // Distancias para la logica de recogida
    [SerializeField] private GameObject selectedChicken; // Pollo seleccionado m�s cercano

    [Header("Music Manager")]
    [SerializeField] private MusicManager musicManager; // Gestor de m�sica y sonidos

    [Header("Game Mode Settings")]
    [SerializeField] private bool gameModeFrenezzi = false; // Modo de juego Frenezzi

    [Header("Colliders")]
    private CapsuleCollider capsuleCollider;
    private BoxCollider boxCollider;
    private bool enableBoxCollider = false;
    private bool isInRange = false;
    private Animator anim; // Controlador de animaciones

    private Dictionary<int, string> chickenVFXEffects = new Dictionary<int, string>
    {
        { 1, "WhiteMuzzle" },  // Pollo normal
        { 2, "RedMuzzle" },   // Pollo grande
        { 3, "RedMuzzle" },   //Pollo Bomba
        { 4, "RedMuzzle" }, // Pollo R�pido
        { 5, "RedMuzzle" },   // Pollo Disparo
        { 6, "RedMuzzle" }  // Pollo curativo
    };

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        boxCollider = GetComponent<BoxCollider>();
        currentProjectile = new List<GameObject>();
    }

    private void LateUpdate()
    {
        DetectNearbyChickens();
    }

    private void Start()
    {
        musicManager = FindAnyObjectByType<MusicManager>();
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void DetectNearbyChickens()
    {
        Vector3 detectionCenter = transform.position + Vector3.up * 1f; // Ajustar 1f dependiendo de la altura del jugador
        Collider[] hitColliders = Physics.OverlapSphere(detectionCenter, pickUpRange); // Usamos OverlapSphere para detectar todos los objetos dentro del rango desde la posicion ajustada
        bool chickenDetected = false; // Bandera para saber si hemos detectado un pollo

        foreach (var hitCollider in hitColliders)
        {
            // Verificamos si el collider tiene el tag "CorpseCollider"
            // Si encontramos un pollo, lo seleccionamos como el pollo mas cercano
            if (hitCollider.CompareTag("CorpseCollider"))
            {
                selectedChicken = hitCollider.GetComponentInParent<ChickenCorpse>().gameObject;
                isInRange = true; // Marcamos que hay un pollo en rango
                chickenDetected = true; // Hemos detectado un pollo

                // Verificamos si el jugador ya tiene un pollo en la mano
                // Si ya tienes un pollo, no hacemos nada, salimos de la funcion
                if (currentChickenType > 0)
                    return;

                // Llamamos al metodo HandleCorpseCollision para gestionar la recogida del pollo
                HandleCorpseCollision(hitCollider);

                break; // Salimos despues de encontrar el primer pollo
            }
        }

        // Si no encontramos ningun pollo, desmarcamos el estado
        if (!chickenDetected)
        {
            isInRange = false;
            selectedChicken = null;
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

            // Llamamos a la funcion para reemplazarlo con el pollo que esta en el suelo
            ReplaceChickenWithNew(selectedChicken);

            // Destruimos el pollo en el suelo
            Destroy(selectedChicken);

            // Desmarcamos el estado de estar en rango
            isInRange = false;
            selectedChicken = null;
        }
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

    public void OnShoot(InputValue value)
    {
        if (currentChickenType > 0)
        {
            anim.SetBool("Carrying", false);
            anim.SetTrigger("Throw");
            musicManager.Play_FX_RecogerPollo();
            Shoot(currentChickenType);
        }
    }

    private void Shoot(int ammoType)
    {
        enableBoxCollider = false;

        if (currentChickenType != 0)
        {
            Vector3 projectilePos = CalculateProjectileStartPosition();

            if (currentChickenType == 6) // Pollo que da vida extra
                playerHealth.IncreaseHealth(1);

            if (!gameModeFrenezzi)
            {
                int chickenTypeBeforeLaunch = currentChickenType;  // Guardamos el tipo antes de lanzarlo

                HandleProjectileLaunch(ammoType, projectilePos);

                // Verifica antes de llamar el efecto
                // Debug.Log($"Intentando reproducir efecto para tipo: {chickenTypeBeforeLaunch}");

                if (chickenVFXEffects.TryGetValue(chickenTypeBeforeLaunch, out string effectName)) // Busca si el tipo de pollo tiene un efecto
                {
                    VFXManager.Instance.PlayEffect(effectName, transform, new Vector3(0.09000015f, 0.8f, 0.2800007f), Quaternion.Euler(0f, 0f, 0f));
                }
                else
                    Debug.LogWarning($"No hay efecto asociado para el tipo de pollo {chickenTypeBeforeLaunch}");

                ClearProjectiles();
            }
            else
                HandleFrenezziMode();
        }
    }

    public void autoShoot(Vector3 targetPosition)
    {
        enableBoxCollider = false;

        if (currentChickenType != 1 && currentChickenType != 4) return;

        Vector3 projectilePos = CalculateProjectileStartPosition();

        if (currentChickenType == 6)
            playerHealth.IncreaseHealth(1);

        if (!gameModeFrenezzi)
        {
            int chickenTypeBeforeLaunch = currentChickenType;
            Vector3 direction = (targetPosition - projectilePos).normalized;

            autoHandleProjectileLaunch(currentChickenType, projectilePos, targetPosition);

            if (chickenVFXEffects.TryGetValue(chickenTypeBeforeLaunch, out string effectName))
            {
                VFXManager.Instance.PlayEffect(effectName, transform,
                    new Vector3(0.09000015f, 0.8f, 0.2800007f), Quaternion.identity);
            }
            else
            {
                Debug.LogWarning($"No hay efecto asociado para el tipo de pollo {chickenTypeBeforeLaunch}");
            }

            ClearProjectiles();
        }
        else
        {
            HandleFrenezziMode();
        }
    }

    private void autoHandleProjectileLaunch(int ammoType, Vector3 projectilePos, Vector3 targetPosition)
    {
        if (ammoType <= 0 || ammoType > proyectiles.Length) return;

        GameObject projectile = Instantiate(proyectiles[ammoType - 1], projectilePos, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        // Ignorar colisión con el jugador
        Collider projCol = projectile.GetComponent<Collider>();
        Collider playerCol = GetComponent<Collider>();
        if (projCol != null && playerCol != null)
            Physics.IgnoreCollision(projCol, playerCol);

        // Direccion hacia el target
        Vector3 direction = (targetPosition - projectilePos).normalized;

        // Aplicar fuerza según tipo de pollo
        float force = proyectileForce;
        if (ammoType == 2) force = bigChickenImpulseForce;
        else if (ammoType == 4) force *= 1.5f;

        rb.AddForce(direction * force, ammoType == 2 ? ForceMode.Impulse : ForceMode.Force);

        musicManager.Play_FX_PLayer_DispararPollo();
        currentChickenType = 0;
    }


    public void ThrowCurrentChicken() //PRUEBA DE LANZAR CON TÁCTIL
    {
        // Solo si tienes un pollo en la mano
        if (currentChickenType > 0)
        {
            // Desactivamos la animación de 'cargando'
            anim.SetBool("Carrying", false);
            // Disparamos la animación de lanzamiento
            anim.SetTrigger("Throw");
            // Sonido de coger/pichar pollo
            musicManager.Play_FX_RecogerPollo();
            // Lógica de instanciar el proyectil o ragdoll
            Shoot(currentChickenType);
        }
    }

    /// <summary>
    /// Requiere que previamente se haya creado un objeto de CurrentWeapon
    /// </summary>
    /// <param name="chickenNumber"></param>
    private void RetrieveChicken(int chickenNumber)
    {
        anim.SetBool("Carrying", true);
        currentChickenType = chickenNumber;

        if (currentChickenType >= 10 || currentChickenType < 0)
            currentChickenType = 1;

        if (currentWeapon != null)
        {
            if (currentProjectile != null)
                if (currentProjectile.Count == 0)
                    currentProjectile.Add(currentWeapon);
                else
                    currentProjectile[0] = currentWeapon;
        }
    }

    public void ReplaceChickenWithNew(GameObject newChicken)
    {
        // Destruimos el pollo actual que el jugador tiene en la mano (si existe)
        if (currentWeapon != null)
            Destroy(currentWeapon);

        currentChickenType = 0; // Restablecemos el tipo de pollo en la mano

        // Obtenemos el tipo de pollo del nuevo pollo que se ha recogido
        int chickenType = newChicken.GetComponent<ChickenCorpse>().chickenType;

        // Creamos el nuevo pollo en la mano del jugador
        GameObject weaponChicken = ragdolls[chickenType - 1];  // Usamos el prefab correspondiente al tipo de pollo
        currentWeapon = Instantiate(weaponChicken, handPosition.position, Quaternion.Euler(transform.rotation.eulerAngles), transform);

        // Actualizamos el tipo de pollo en la mano
        RetrieveChicken(chickenType);

        // Destruimos el pollo en el suelo despues de que lo hayamos recogido
        //  Destroy(newChicken);  // Este es el pollo del suelo
    }

    /// <summary>
    /// Limpia la lista de proyectiles
    /// </summary>
    private void ClearProjectiles()
    {
        if (currentProjectile != null)
        {
/*

            GameObject auxProjectile = currentProjectile[0];
            currentProjectile.Clear();

            if (auxProjectile != null)
            {
                Destroy(auxProjectile);
            }
*/
            // Destruir cada proyectil
            foreach (var proj in currentProjectile)
                if (proj != null)
                    Destroy(proj);

            // Vaciar la lista
            currentProjectile.Clear();
        }

        currentWeapon = null;
        currentChickenType = 0;
    }

    public void PerformAttack(int ammoType)
    {
        if (ammoType == 0)
            HeadBut();
        else if (ammoType >= 1)
            ChickenSwing();
    }

    public void HeadBut()
    {
        if (headBox != null)
            StartCoroutine(ActivateColliderChicken(headBox));
    }

    public void ChickenSwing()
    {
        if (swingBox != null)
            StartCoroutine(ActivateCollider(swingBox));
    }

    private IEnumerator ActivateCollider(GameObject collider)
    {
        yield return new WaitForSeconds(0.2f);
        collider.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        collider.SetActive(false);
    }

    private IEnumerator ActivateColliderChicken(GameObject collider)
    {
        yield return new WaitForSeconds(0.15f);
        collider.SetActive(true);
        enableBoxCollider = true;
        yield return new WaitForSeconds(0.2f);
        collider.SetActive(false);
    }

    private void HandleCorpseCollision(Collider other)
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
                Destroy(selectedChicken);

            if (currentWeapon != null)
                Destroy(currentWeapon);

            currentWeapon = Instantiate(weaponChicken, handPosition.position, Quaternion.Euler(transform.rotation.eulerAngles), transform);
            RetrieveChicken(type); // Actualizar el tipo de pollo en la mano
            enableBoxCollider = false;
        }
    }

    private Vector3 CalculateProjectileStartPosition()
    {
        if (currentChickenType == 2)
            return transform.position + (transform.forward * 2) + (transform.up * 2);
        else
            return transform.position + transform.forward + transform.up;
    }

    private void HandleProjectileLaunch(int ammoType, Vector3 projectilePos)
    {
        if (ammoType > 0 && ammoType <= proyectiles.Length)
        {
            GameObject projectile = Instantiate(proyectiles[ammoType - 1], projectilePos, Quaternion.identity);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            // ← AÑADIR: Ignorar colisión con el jugador
            Collider projectileCollider = projectile.GetComponent<Collider>();
            Collider playerCollider = GetComponent<Collider>(); // suponiendo que este script está en el jugador

            if (projectileCollider != null && playerCollider != null)
                Physics.IgnoreCollision(projectileCollider, playerCollider);

            // Final AÑADIDO

            if (currentChickenType == 2)
                rb.AddForce(transform.forward * bigChickenImpulseForce, ForceMode.Impulse); //darle físicas al pollo bola
            else if (currentChickenType == 4)
                rb.AddForce(transform.forward * proyectileForce * 1.5f);
            else
                rb.AddForce(transform.forward * proyectileForce);

            musicManager.Play_FX_PLayer_DispararPollo();
            currentChickenType = 0;
        }
    }

    private void HandleFrenezziMode()
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

}
