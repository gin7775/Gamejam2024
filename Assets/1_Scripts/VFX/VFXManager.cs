using UnityEngine;
using System.Collections.Generic;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;
    [SerializeField] private List<VFXEffect> effects; // Lista de efectos configurados en el editor.
    private Dictionary<string, VFXEffect> effectDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            effectDictionary = new Dictionary<string, VFXEffect>();

            foreach (var effect in effects)
            {
                effectDictionary.Add(effect.EffectName, Instantiate(effect)); // ✅ siempre instanciamos una copia
                effectDictionary[effect.EffectName].gameObject.SetActive(false); // Para no dejarla visible
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayEffect(string effectName, Transform parent, Vector3 localPosition, Quaternion localRotation)
    {
        if (!effectDictionary.TryGetValue(effectName, out VFXEffect prefab))
        {
            Debug.LogWarning($"[VFXManager] Efecto '{effectName}' no encontrado.");
            return;
        }

        // Instanciar como hijo del transform deseado
        GameObject instance = Instantiate(prefab.gameObject);
        instance.name = effectName;

        // Establecer como hijo (mismo parent que antes)
        instance.transform.SetParent(parent);

        // Restaurar la escala del prefab
        instance.transform.localScale = prefab.transform.localScale;

        // Colocar en la posición y rotación local correctas
        instance.transform.localPosition = localPosition;
        instance.transform.localRotation = localRotation;

        // ⚠️ Mover a capa segura si quieres evitar colisiones
        instance.layer = LayerMask.NameToLayer("Ignore Raycast");

        // Desactivar colliders para evitar interferencias físicas
        foreach (var col in instance.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
        foreach (var col2D in instance.GetComponentsInChildren<Collider2D>())
        {
            col2D.enabled = false;
        }

        // Activar el objeto
        instance.SetActive(true);

        // Reproducir efecto visual
        var vfx = instance.GetComponent<VFXEffect>();
        if (vfx != null) vfx.Play();

        // Destruir tras 5 segundos
        Destroy(instance, 5f);
    }

}
