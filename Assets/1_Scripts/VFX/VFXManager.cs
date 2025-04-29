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
        if (effectDictionary == null)
        {
            Debug.LogError("VFXManager: effectDictionary is null. Did Awake() run");
            return;
        }

        if (effectDictionary.TryGetValue(effectName, out var effect) && effect != null)
        {
            var instance = effect.GetInstance();
            if (instance == null)
            {
                Debug.LogWarning("VFXManager: effect '{effectName}' returned null instance.");
                return;
            }

            instance.transform.SetParent(parent);
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = localRotation;
            instance.Play();
        }
        else
        {
            Debug.LogWarning("Effect '{effectName}' not found in dictionary!");
        }
    }

}
