using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movimiento_Enemigo1 : StateMachineBehaviour
{
    
    NavMeshAgent enemy;
    ContenedorEnemigo1 contenedorEnemy;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject.GetComponent<NavMeshAgent>();
        
        contenedorEnemy = animator.gameObject.GetComponent<ContenedorEnemigo1>();
        enemy.speed = 3.5f;
        //contenedorEnemy.animEnemy.SetTrigger("Walk");
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            enemy.destination = player.transform.position;
            if (Vector3.Distance(enemy.transform.position, player.transform.position) <= contenedorEnemy.distanceToEnemy)
            {
                animator.SetTrigger("Ataque");
                enemy.speed = 0f;


            }

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
