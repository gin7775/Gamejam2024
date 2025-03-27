using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float destroyTime = 5f; // Tiempo en segundos

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}