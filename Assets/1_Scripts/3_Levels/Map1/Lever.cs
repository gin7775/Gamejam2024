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
    public GameObject ColliderCanPass;
    public GameObject ColliderCanNotPass;

    private void Start()
    {
        if(blocked)
        {
            CannotPass();
        }
    }
    public void Interact()
    {
        if (!canInteract) return;

        if (blocked)
        {
            CanPass(); 
        }
        else
        {
            CannotPass();
        }

        canInteract = false;
        Invoke(nameof(ResetInteraction), interactionCooldown);
    }

    void CanPass()
    {
        obstacle.SetActive(false);
        blocked = false;
        bridge1ForAnimation.GetComponent<Animator>().SetTrigger("Lever");
        bridge2ForAnimation.GetComponent<Animator>().SetTrigger("Lever");
        ColliderCanNotPass.SetActive(false);
        ColliderCanPass.SetActive(true);

        Debug.Log("Se puede cruzar " + this.gameObject.name);
    }

    void CannotPass ()
    {
        obstacle.SetActive(true);
        blocked = true;
        bridge1ForAnimation.GetComponent<Animator>().SetTrigger("Lever");
        bridge2ForAnimation.GetComponent<Animator>().SetTrigger("Lever");
        ColliderCanNotPass.SetActive(true);
        ColliderCanPass.SetActive(false);
        
        Debug.Log("No se puede cruzar " + this.gameObject.name);
    }

    void ResetInteraction()
    {
        canInteract = true;
    }

}
