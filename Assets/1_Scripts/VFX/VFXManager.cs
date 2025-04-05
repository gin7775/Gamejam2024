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
                effectDictionary.Add(effect.EffectName, effect);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayEffect(string effectName, Transform parent, Vector3 localPosition, Quaternion localRotation)
    {
        if (effectDictionary.TryGetValue(effectName, out var effect))
        {
            //Debug.Log($"Reproduciendo efecto: {effectName}");
            var instance = effect.GetInstance();
            instance.transform.SetParent(parent);
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = localRotation;
            instance.Play();
        }
        else
        {
            Debug.LogWarning($"Effect {effectName} not found in dictionary!");
        }
    }

}
