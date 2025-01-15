using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;
    private Transform playerTransform; // Player transform

    [Header("Shoot")]
    [SerializeField] private GameObject projectilePrefab; // Prefab del proyectil
    [SerializeField] private Transform shootingPoint; // Punto de origen del disparo
    [SerializeField] private float projectileSpeed = 10f; // Velocidad del proyectil
    [SerializeField] private float fireRate = 2f; // Frecuencia de disparo (Disparos por segundo)
    [SerializeField] private float fireCooldown; // Controla el tiempo entre disparos
    //[SerializeField] private float stopDistance = 10f; // Distancia mínima para detenerse y disparar
    //[SerializeField] private float moveDistance = 15f; // Distancia máxima para moverse hacia el jugador

    [Header("Explosions")]
    [SerializeField] private GameObject explosionAreaPrefab; // Prefab del área de daño
    [SerializeField] private float explosionRadius = 5f; // Radio de las explosiones
    [SerializeField] private int explosionsPerBatch = 3; // Cantidad de explosiones por tanda
    [SerializeField] private float repeatInterval = 5f; // Tiempo entre tandas de explosiones
    [SerializeField] private float minimumDistanceBetweenExplosions = 2f; // Distancia mínima entre explosiones


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        SpawnExplosionsAroundPlayer();
    }

    // Update is called once per frame

    void Update()
    {
        if (playerTransform == null) return;

        // Actualizar el cooldown para disparar
        fireCooldown -= Time.deltaTime;

        // Calcular la distancia al jugador
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Mirar hacia el jugador
        Vector3 lookPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        transform.LookAt(lookPosition);

        // Disparar siempre que esté quieto y el cooldown ha terminado
        if (fireCooldown <= 0f)
        {
            ShootAtPlayer();
            fireCooldown = 1f / fireRate; // Reiniciar el cooldown basado en la tasa de disparo
        }

    }

    // ---------------- Movement ----------------



    // ---------------- ShootProjectile ----------------

    private void ShootAtPlayer()
    {
        if (playerTransform != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);

            // Obtener la dirección hacia el jugador en el momento del disparo
            Vector3 directionToPlayer = playerTransform.position - shootingPoint.position;

            // Eliminar la componente Y para asegurar un disparo plano
            directionToPlayer.y = 0f;

            // Normalizar la dirección para que tenga longitud 1
            directionToPlayer = directionToPlayer.normalized;

            // Aplicar movimiento al proyectil
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = directionToPlayer * projectileSpeed;
            }
        }
    }

    // ---------------- ExplosionMETHODS ----------------

    public void SpawnExplosionsAroundPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("No hay referencia al jugador.");
            return;
        }

        // Iniciar la coroutine para generar explosiones recurrentes
        StartCoroutine(RepeatedExplosionsCoroutine());
    }

    private IEnumerator RepeatedExplosionsCoroutine()
    {
        while (true) // Bucle infinito para explosiones recurrentes
        {
            // Generar una tanda de explosiones alrededor del jugador
            SpawnExplosionsBatch();

            // Esperar antes de repetir la tanda
            yield return new WaitForSeconds(repeatInterval);
        }
    }

    private void SpawnExplosionsBatch()
    {
        if (player == null)
        {
            Debug.LogWarning("No hay referencia al jugador.");
            return;
        }

        // Detectar la posición actual del jugador
        Vector3 playerPos = player.transform.position;

        // Lista temporal para esta tanda de explosiones
        List<Vector3> generatedPositions = new List<Vector3>();

        for (int i = 0; i < explosionsPerBatch; i++) // `explosionsPerBatch` controla cuántas explosiones generar
        {
            Vector3 spawnPosition;
            bool isValidPosition;

            do
            {
                // Generar una posición aleatoria alrededor del jugador
                Vector2 randomPos2D = Random.insideUnitCircle * explosionRadius;
                spawnPosition = new Vector3(playerPos.x + randomPos2D.x, playerPos.y, playerPos.z + randomPos2D.y);

                // Validar que la posición no esté demasiado cerca de las ya generadas
                isValidPosition = true;
                foreach (var pos in generatedPositions)
                {
                    if (Vector3.Distance(spawnPosition, pos) < minimumDistanceBetweenExplosions)
                    {
                        isValidPosition = false;
                        break;
                    }
                }

            } while (!isValidPosition);

            // Añadir la posición generada a la lista
            generatedPositions.Add(spawnPosition);

            // Instanciar el área de daño
            GameObject explosionArea = Instantiate(explosionAreaPrefab, spawnPosition, Quaternion.identity);
        }
    }

}
