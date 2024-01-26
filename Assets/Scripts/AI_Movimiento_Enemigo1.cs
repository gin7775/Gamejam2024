using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movimiento_Enemigo1 : StateMachineBehaviour
{
    Transform destination;
    NavMeshAgent enemy;
    ContenedorEnemigo1 contenedorEnemy;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject.GetComponent<NavMeshAgent>();
        destination = animator.gameObject.GetComponent<ContenedorEnemigo1>().player;
        contenedorEnemy = animator.gameObject.GetComponent<ContenedorEnemigo1>();
        //contenedorEnemy.animEnemy.SetTrigger("Walk");
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.speed = 3.5f;
        enemy.destination = destination.position;
        if (Vector3.Distance(enemy.transform.position, destination.position) <= contenedorEnemy.distanceToEnemy)
        {
            animator.SetTrigger("Ataque");
            //contenedorEnemy.animEnemy.SetTrigger("Attack");

        }
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
