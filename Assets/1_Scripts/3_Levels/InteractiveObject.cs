using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField] private MonoBehaviour interactableScript;

    private IInteractable interactable;
    public bool canBeActivatedByImpacts = false;

    private void Start()
    {
        // Verificamos si el script asignado implementa la interfaz IInteractable
        interactable = interactableScript as IInteractable;
        if (interactable == null)
        {
            Debug.LogError("El script asignado no implementa la interfaz IInteractable.");
        }
    }

    public void ExecuteInteraction()
    {
        if (interactable != null)
        {
            interactable.Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canBeActivatedByImpacts && "Projectile".Equals(other.gameObject.tag))
        {
            if (interactable != null)
            {
                interactable.Interact();
            }
        }

    }
}
