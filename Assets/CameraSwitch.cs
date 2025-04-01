using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private Animator cinemachineAnim;
    [SerializeField] private string cameraAAnim = "";
    [SerializeField] private string cameraBAnim = "";
    public GameObject OptionsMenu;

    private bool usingCameraA = false;

    public void SetCameraA()
    {
       
            cinemachineAnim.Play(cameraAAnim);
            cinemachineAnim.SetTrigger("ActiveA");
            usingCameraA = true;
            Debug.Log("Cambiada a Cámara A");
      
    }

    public void SetCameraB()
    {
        
            cinemachineAnim.Play(cameraBAnim);
            usingCameraA = false;
            cinemachineAnim.SetTrigger("ActiveB");
            Debug.Log("Volviendo a Cámara B");
      
    }

    // Si querés hacer un toggle (opcional)
    public void ToggleCamera()
    {
        if (usingCameraA)
            SetCameraB();
        else
            SetCameraA();
    }

    public void ActiveGameObject()
    {
        OptionsMenu.SetActive(true);
    }
}
