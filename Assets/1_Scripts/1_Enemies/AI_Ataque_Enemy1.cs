using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Ataque_Enemy1 : StateMachineBehaviour
{
    NavMeshAgent enemy;
    ContenedorEnemigo1 contenedorEnemy;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject.GetComponent<NavMeshAgent>();
        contenedorEnemy = animator.gameObject.GetComponent<ContenedorEnemigo1>();
        //contenedorEnemy.animEnemy.SetTrigger("Attack");

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); 
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, animator.transform.position);
            if (distanceToPlayer >= contenedorEnemy.distanceToEnemy)
            {
                contenedorEnemy.animEnemy.SetTrigger("move"); //la animación

                animator.SetTrigger("Movimiento");

            }
           
        }

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
