using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTouchWater : MonoBehaviour
{
     [Tooltip("Arrastra aquí el Transform de la posición de reset")]
    [SerializeField] private Transform resetPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var controller = other.GetComponent<CharacterController>();
        if (controller == null) return;

        controller.enabled = false;

        // Posición
        other.transform.position = resetPoint.position;
        // Rotación: alineamos el forward del jugador con el del resetPoint
        other.transform.rotation = Quaternion.LookRotation(resetPoint.forward);

        controller.enabled = true;
    }


}
