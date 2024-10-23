using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movimiento_Enemigo2 : StateMachineBehaviour
{
    NavMeshAgent enemy;
    ContenedorEnemigo1 contenedorEnemy;
    Animator animatorHijo;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject.GetComponent<NavMeshAgent>();
        contenedorEnemy = animator.gameObject.GetComponent<ContenedorEnemigo1>();
        enemy.speed = contenedorEnemy.speed;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && player.layer != LayerMask.NameToLayer("Invisible"))
        {
            enemy.destination = player.transform.position;
            if (Vector3.Distance(enemy.transform.position, player.transform.position) <= contenedorEnemy.distanceToEnemy)
            {
                contenedorEnemy.animEnemy.SetTrigger("Attack"); //la animación
                animator.SetTrigger("Ataque"); //la IA
                enemy.speed = 0.3f;
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

}
