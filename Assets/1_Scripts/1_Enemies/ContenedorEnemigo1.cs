using UnityEngine;
using UnityEngine.AI;

public class ContenedorEnemigo1 : MonoBehaviour
{
    [SerializeField] public int lifes = 3;
    public float speed;
    public float distanceToAttack = 1f;

    public Animator animEnemy;
    public GameObject playerReference;
    public int enemyTipe;
    public float distanceToEnemy;
    public Transform destination;
    
    public GameObject corpse;
    public bool canDamage;

    // Variables para la corriente de agua
    private Vector3 waterCurrentDirection = Vector3.zero;
    private float waterCurrentForce = 0f;
    private bool inWaterCurrent = false;
    public float currentForceMultiplier = 0.3f;

    public void Start()
    {
        canDamage = true;
        playerReference = GameObject.FindGameObjectWithTag("Player");
    }

    // Actualiza la dirección de movimiento del NavMeshAgent teniendo en cuenta la corriente
    public void UpdatePositionWithCurrent(NavMeshAgent enemy)
    {
        // Si el enemigo está en la corriente de agua, aplica la dirección de la corriente
        if (inWaterCurrent)
        {
            Vector3 desiredMoveDirection = (enemy.destination - enemy.transform.position).normalized;
            float dotProduct = Vector3.Dot(desiredMoveDirection.normalized, waterCurrentDirection.normalized);

            Vector3 currentEffect = (dotProduct < 0)
             ? waterCurrentDirection * (waterCurrentForce * currentForceMultiplier)
             : waterCurrentDirection * (waterCurrentForce * currentForceMultiplier);

            // Aplica el efecto de la corriente
            enemy.velocity += currentEffect;  // Cambia la dirección del NavMeshAgent
        }
    }

    // Función para aplicar la corriente de agua
    public void ApplyWaterCurrent(Vector3 direction, float force)
    {
        waterCurrentDirection = direction;
        waterCurrentForce = force;
        inWaterCurrent = true;
    }

    // Función para remover la corriente de agua
    public void RemoveWaterCurrent()
    {
        inWaterCurrent = false;
        waterCurrentDirection = Vector3.zero;
        waterCurrentForce = 0f;
    }

    public void PolloMansy()
    {
        GameObject auxCorpse = GameObject.Instantiate(corpse, transform.position, Quaternion.identity);
        GameManager.Instance.listCorpses.Add(auxCorpse);
        CorpseManager.Instance.addCorpseTimeLife(auxCorpse);
    }
}
