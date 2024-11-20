using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject obstacle;
    public bool blocked = false;
    private bool canInteract = true;
    public float interactionCooldown = 2f;

    public GameObject bridge1ForAnimation;
    public GameObject bridge2ForAnimation;
    public GameObject bridgeColliderOpen;
    public GameObject bridgeColliderClosed;

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
        bridge1ForAnimation.GetComponent<Animator>().SetTrigger("Lever");
        bridge2ForAnimation.GetComponent<Animator>().SetTrigger("Lever");
        bridgeColliderClosed.SetActive(false);
        bridgeColliderOpen.SetActive(true);
        Debug.Log("Puente abierto");
    }

    void Close()
    {
        obstacle.SetActive(true);
        blocked = true;
        bridge1ForAnimation.GetComponent<Animator>().SetTrigger("Lever");
        bridge2ForAnimation.GetComponent<Animator>().SetTrigger("Lever");
        bridgeColliderClosed.SetActive(true);
        bridgeColliderOpen.SetActive(false);
        Debug.Log("Puente cerrado");
    }

    void ResetInteraction()
    {
        canInteract = true;
    }

}
