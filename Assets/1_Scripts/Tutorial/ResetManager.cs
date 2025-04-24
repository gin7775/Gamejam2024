using UnityEngine;

public class ResetManager : MonoBehaviour
{
    public static ResetManager Instance { get; private set; }

    [Tooltip("Lista ordenada de puntos de spawn")]
    [SerializeField] private Transform[] spawnPoints;
    private Transform currentSpawn;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (spawnPoints.Length > 0)
            currentSpawn = spawnPoints[0];
    }

    public Transform GetCurrentSpawn() => currentSpawn;

    public void SetCheckpoint(Transform newSpawn)
    {
        foreach (var p in spawnPoints)
            if (p == newSpawn)
            {
                currentSpawn = p;
                Debug.Log($"Checkpoint actualizado a {p.name}");
                break;
            }
    }
}