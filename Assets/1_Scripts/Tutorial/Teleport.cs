using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var controller = other.GetComponent<CharacterController>();
        if (controller == null) return;

        var resetPoint = ResetManager.Instance.GetCurrentSpawn();
        if (resetPoint == null) return;

        controller.enabled = false;
        other.transform.position = resetPoint.position;
        other.transform.rotation = Quaternion.LookRotation(resetPoint.forward);
        controller.enabled = true;
        Debug.Log("Teleported");
    }


}
