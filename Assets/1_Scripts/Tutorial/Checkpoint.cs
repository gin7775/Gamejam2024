using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{

    public Transform spawnCheckPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        ResetManager.Instance.SetCheckpoint(spawnCheckPoint);
        Debug.Log("Checkpoint");
    }
}
