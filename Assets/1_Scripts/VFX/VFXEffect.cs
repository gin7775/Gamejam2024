using System.Collections.Generic;
using UnityEngine;

public class VFXEffect : MonoBehaviour
{
    [SerializeField] private string effectName; // Nombre del efecto (por ejemplo, "MuzzleFlash").
    [SerializeField] private ParticleSystem particleSystem; // Sistema de partículas asociado.
    [SerializeField] private bool usePooling = true;

    private static Dictionary<string, Queue<VFXEffect>> poolDictionary;

    public string EffectName => effectName;

    private void Awake()
    {
        if (usePooling)
        {
            if (poolDictionary == null)
            {
                poolDictionary = new Dictionary<string, Queue<VFXEffect>>();
            }

            if (!poolDictionary.ContainsKey(effectName))
            {
                poolDictionary.Add(effectName, new Queue<VFXEffect>());
            }
        }
    }

    public VFXEffect GetInstance()
    {
        if (usePooling && poolDictionary.TryGetValue(effectName, out var pool) && pool.Count > 0)
        {
            var instance = pool.Dequeue();
            instance.gameObject.SetActive(true);
            Debug.Log($"Instanciando desde el pool: {effectName}");
            return instance;
        }

        Debug.Log($"Instanciando nuevo efecto: {effectName}");
        return Instantiate(this);
    }

    public void ReturnToPool()
    {
        if (usePooling && poolDictionary.TryGetValue(effectName, out var pool))
        {
            Stop();
            transform.SetParent(null); // Desvinculamos el efecto del jugador
            gameObject.SetActive(false);
            pool.Enqueue(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Play()
    {
        // Resetear el sistema de partículas
        particleSystem.Clear();
        particleSystem.Stop();
        particleSystem.Play();

        // Calcular la duración del efecto (la duración total)
        float duration = particleSystem.main.duration + particleSystem.main.startLifetime.constantMax;

        // Devolver al pool después de que termine el efecto
        Invoke(nameof(ReturnToPool), duration);
    }

    public void Stop()
    {
        particleSystem.Stop();
        particleSystem.Clear();
    }
}
