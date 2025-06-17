using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [Tooltip("Tiempo en segundos antes de destruir este objeto")]
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        // Destruye este GameObject tras 'lifetime' segundos
        Destroy(gameObject, lifetime);
    }
}
