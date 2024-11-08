using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject obstacle;
    public bool blocked = false;
    private bool canInteract = true;
    public float interactionCooldown = 2f;

    public void Interact()
    {
        if (!canInteract) return;

        if (blocked)
        {
            Open();
        }
        else
        {
            Close();
        }

        canInteract = false;
        Invoke(nameof(ResetInteraction), interactionCooldown);
    }

    void Open()
    {
        obstacle.SetActive(false);
        blocked = false;
        Debug.Log("Puente abierto");
    }

    void Close()
    {
        obstacle.SetActive(true);
        blocked = true;
        Debug.Log("Puente cerrado");
    }

    void ResetInteraction()
    {
        canInteract = true;
    }

}
