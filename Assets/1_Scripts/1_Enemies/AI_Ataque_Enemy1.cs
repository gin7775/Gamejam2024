using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Ataque_Enemy1 : StateMachineBehaviour
{
    NavMeshAgent enemy;
    ContenedorEnemigo1 contenedorEnemy;
    private GameObject player;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject.GetComponent<NavMeshAgent>();
        contenedorEnemy = animator.gameObject.GetComponent<ContenedorEnemigo1>();
        player = GameObject.FindGameObjectWithTag("Player");
        //contenedorEnemy.animEnemy.SetTrigger("Attack");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


        if (player != null)
        {
            // Rotar hacia el jugador
            Vector3 direction = (player.transform.position - enemy.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * 10f);

            // Evaluar la distancia
            float distanceToPlayer = Vector3.Distance(player.transform.position, animator.transform.position);
            if (distanceToPlayer >= contenedorEnemy.distanceToEnemy)
            {
                contenedorEnemy.animEnemy.SetTrigger("move");
                animator.SetTrigger("Movimiento");
            }
        }



        //// Forzar al enemigo a mirar al jugador
        //Vector3 direction = (player.transform.position - enemy.transform.position).normalized;
        //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * 10f);


        //if (player != null)
        //{
        //    float distanceToPlayer = Vector3.Distance(player.transform.position, animator.transform.position);
        //    if (distanceToPlayer >= contenedorEnemy.distanceToEnemy)
        //    {
        //        contenedorEnemy.animEnemy.SetTrigger("move"); //la animación

        //        animator.SetTrigger("Movimiento");
        //    }
        //}
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
