using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BridgeRandomMovers : MonoBehaviour
{
    [SerializeField]
    private GameObject[] targetObjects; // Array de objetos con Animator

    [SerializeField]
    private float triggerInterval = 2f; // Intervalo de tiempo en segundos

    public bool TwoRandoms = false;

    //public const string triggerName = "Lever"; // Nombre del Trigger en los Animators

    void Start()
    {
        // Iniciar la corrutina para activar los triggers periódicamente
        StartCoroutine(ActivateAnimatorTrigger());
    }

    private IEnumerator ActivateAnimatorTrigger()
    {
        if(TwoRandoms == false)
        {
            while (true)
            {
                if (targetObjects.Length > 0)
                {
                    // Seleccionar un índice aleatorio
                    int randomIndex = Random.Range(0, targetObjects.Length);

                    GameObject selectedObject = targetObjects[randomIndex];

                    // Verificar si el objeto no es nulo y tiene el componente Lever
                    if (selectedObject != null)
                    {
                        Lever lever = selectedObject.GetComponent<Lever>();
                        if (lever != null)
                        {
                            lever.Interact();
                        }
                        else
                        {
                            Debug.LogWarning($"El objeto {selectedObject.name} no tiene un componente Lever.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"El índice {randomIndex} del arreglo targetObjects es nulo.");
                    }
                }
                else
                {
                    Debug.LogWarning("El arreglo targetObjects está vacío.");
                }

                // Esperar el intervalo definido
                yield return new WaitForSeconds(triggerInterval);
            }
        }

        if (TwoRandoms) 
        {
            while (true)
            {
                foreach (GameObject obj in targetObjects)
                {

                    obj.GetComponent<Lever>().Interact();

                }
                yield return new WaitForSeconds(triggerInterval);
            }
        }



    }

}
