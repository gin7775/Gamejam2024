using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeRandomMovers : MonoBehaviour
{
    [SerializeField]
    private GameObject[] targetObjects; // Array de objetos que tienen un Lever

    [SerializeField]
    private float triggerInterval = 2f; // Intervalo de tiempo en segundos

    void Start()
    {
        // Ajustar el estado inicial para asegurar 2 puentes abiertos y 1 cerrado
        AdjustBridgesToTwoOpenOneClosed();

        // Iniciar la corrutina
        StartCoroutine(CycleBridges());
    }

    private IEnumerator CycleBridges()
    {
        while (true)
        {
            // Antes de la siguiente rotación, asegurarse de que estamos en el estado correcto
            AdjustBridgesToTwoOpenOneClosed();

            // Ahora rotar: abrir el que estaba cerrado y cerrar uno de los que estaban abiertos
            RotateBridges();

            yield return new WaitForSeconds(triggerInterval);
        }
    }

    /// <summary>
    /// Ajusta el estado de los puentes para que siempre haya 2 abiertos y 1 cerrado.
    /// Si no se cumple esta condición, realiza las interacciones necesarias.
    /// </summary>
    private void AdjustBridgesToTwoOpenOneClosed()
    {
        List<Lever> openBridges = new List<Lever>();
        List<Lever> closedBridges = new List<Lever>();

        // Separar puentes en abiertos y cerrados
        foreach (GameObject obj in targetObjects)
        {
            Lever lever = obj.GetComponent<Lever>();
            if (lever != null)
            {
                if (!lever.blocked) // !blocked = abierto
                {
                    openBridges.Add(lever);
                }
                else // blocked = cerrado
                {
                    closedBridges.Add(lever);
                }
            }
            else
            {
                Debug.LogWarning($"El objeto {obj.name} no tiene un componente Lever.");
            }
        }

        // Corregir exceso de abiertos
        while (openBridges.Count > 2)
        {
            Lever leverToClose = openBridges[0];
            openBridges.RemoveAt(0);
            leverToClose.Interact(); // Cerrar
            closedBridges.Add(leverToClose);
        }

        // Si hay menos de 2 abiertos, abrir suficientes hasta tener 2
        while (openBridges.Count < 2 && closedBridges.Count > 0)
        {
            Lever leverToOpen = closedBridges[0];
            closedBridges.RemoveAt(0);
            leverToOpen.Interact(); // Abrir
            openBridges.Add(leverToOpen);
        }

        // Si de alguna forma quedan más de un cerrado, cerramos uno solo y abrimos el resto, manteniendo 2 abiertos - 1 cerrado
        while (closedBridges.Count > 1)
        {
            // Abrimos uno más para dejar sólo uno cerrado
            Lever leverToOpen = closedBridges[0];
            closedBridges.RemoveAt(0);
            leverToOpen.Interact();
            openBridges.Add(leverToOpen);
        }

        // En este punto deberíamos tener 2 abiertos y 1 cerrado.
    }

    /// <summary>
    /// Realiza la rotación: cierra uno de los abiertos y abre el que estaba cerrado.
    /// Esto mantiene el patrón pero permite cambio constante.
    /// </summary>
    private void RotateBridges()
    {
        List<Lever> openBridges = new List<Lever>();
        Lever closedBridge = null;

        // Identificar nuevamente el estado actual (debería ser 2 abiertos, 1 cerrado)
        foreach (GameObject obj in targetObjects)
        {
            Lever lever = obj.GetComponent<Lever>();
            if (lever != null)
            {
                if (!lever.blocked)
                    openBridges.Add(lever);
                else
                    closedBridge = lever;
            }
        }

        // Debemos tener 2 abiertos y 1 cerrado
        if (openBridges.Count == 2 && closedBridge != null)
        {
            // Escoger uno de los abiertos para cerrar
            int randomOpenIndex = Random.Range(0, openBridges.Count);
            Lever openToClose = openBridges[randomOpenIndex];
            Lever toOpen = closedBridge;

            // Intercambiar estados
            openToClose.Interact(); // Cerrar el que estaba abierto
            toOpen.Interact();      // Abrir el que estaba cerrado
        }
        else
        {
            // Si no cumple la condición, corregimos
            AdjustBridgesToTwoOpenOneClosed();
        }
    }
}
