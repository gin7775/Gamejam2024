using UnityEngine;
using UnityEngine.AI;

public class AI_Movimiento_Enemigo1 : StateMachineBehaviour
{
    private NavMeshAgent enemy;
    private ContenedorEnemigo1 contenedorEnemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject.GetComponent<NavMeshAgent>();
        contenedorEnemy = animator.gameObject.GetComponent<ContenedorEnemigo1>();
        enemy.speed = contenedorEnemy.speed;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Vector3 targetPosition = player.transform.position;

            // Aplica la dirección de la corriente si el enemigo está en el agua
            contenedorEnemy.UpdatePositionWithCurrent(enemy); // Le pasamos el NavMeshAgent para actualizar la dirección

            enemy.destination = targetPosition;

            if (Vector3.Distance(enemy.transform.position, player.transform.position) <= contenedorEnemy.distanceToEnemy)
            {
                contenedorEnemy.animEnemy.SetTrigger("Attack");
                animator.SetTrigger("Ataque");
                enemy.speed = 0.3f;  // Baja la velocidad al atacar
            }
            else
            {
                enemy.speed = contenedorEnemy.speed; // Restablece la velocidad original
            }
        }
    }
}
