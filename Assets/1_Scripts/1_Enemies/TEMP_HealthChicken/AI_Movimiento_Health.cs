using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movimiento_Health : StateMachineBehaviour
{
    NavMeshAgent enemy;
    ContenedorEnemigo1 contenedorEnemy;
    Animator animatorHijo;
    GameObject player;

    
    float detectionDistance = 5f; // Distancia a la que el NPC detecta al jugador
    float escapeDistance = 10f; // Distancia a la que el NPC se detiene
    float restTime = 3f; // Tiempo de descanso en segundos
    float fleeSpeed = 5f; // Velocidad a la que el NPC huye

    bool isResting = false;
    bool isExhausted = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject.GetComponent<NavMeshAgent>();
        contenedorEnemy = animator.gameObject.GetComponent<ContenedorEnemigo1>();
        enemy.speed = contenedorEnemy.speed;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (player != null && player.layer != LayerMask.NameToLayer("Invisible"))
        {
            //enemy.destination = player.transform.position;

        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

}
