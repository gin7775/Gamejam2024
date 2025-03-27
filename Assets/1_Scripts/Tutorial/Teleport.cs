using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public List<Checkpoint> checkpoints = new List<Checkpoint>();
    public int currentCheckpointIndex = 0;
    public GameObject player;

    void Start()
    {
        if (player == null)
            Debug.LogError("CheckpointManager: No se ha asignado el jugador.");
    }

    // Avanza al siguiente checkpoint
    public void AdvanceCheckpoint()
    {
        if (currentCheckpointIndex < checkpoints.Count - 1)
        {
            currentCheckpointIndex++;
            Debug.Log("Checkpoint avanzado a: " + currentCheckpointIndex);
        }
        else
        {
            Debug.Log("Ya estás en el último checkpoint.");
        }
    }

    // Teletransporta al jugador al checkpoint actual
    public void RespawnAtCheckpoint()
    {
        if (player != null && checkpoints.Count > 0)
        {
            player.transform.position = checkpoints[currentCheckpointIndex].transform.position;
            Debug.Log("Jugador respawneado en checkpoint " + currentCheckpointIndex);
        }
    }

}
