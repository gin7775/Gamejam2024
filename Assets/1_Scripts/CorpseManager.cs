using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseManager : MonoBehaviour
{
    // Singleton
    public static CorpseManager Instance { get; private set; }

    private List<ChickenCorpseTimeLife> listCorpse;
    [SerializeField] private float timeToCorpseLife = 30f;

    // Singleton pattern
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        listCorpse = new List<ChickenCorpseTimeLife>();
    }

    internal void addCorpseTimeLife(GameObject corpseTimeLife)
    {
        listCorpse.Add(new ChickenCorpseTimeLife(corpseTimeLife, Time.time + timeToCorpseLife));
    }

    // Corrutina que se ejecuta en intervalos
    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // Chequea cada 5 segundos
            CheckCorpses();
        }
    }

    private void CheckCorpses()
    {
        List<ChickenCorpseTimeLife> corpsesToRemove = new List<ChickenCorpseTimeLife>();

        foreach (var chickenCorpseTimeLife in listCorpse)
        {
            if (chickenCorpseTimeLife.secondsLife <= Time.time)
            {
                StartCoroutine(HandleCorpseDestruction(chickenCorpseTimeLife)); // Llama a la corrutina para destruir
                corpsesToRemove.Add(chickenCorpseTimeLife); // Agrega a la lista para eliminar
            }
        }

        foreach (var corpse in corpsesToRemove)
        {
            listCorpse.Remove(corpse);
        }
    }

    private IEnumerator HandleCorpseDestruction(ChickenCorpseTimeLife chickenCorpseTimeLife)
    {
        try
        {
            // Aquí localizas las partes del cuerpo
            GameObject bodyOne = chickenCorpseTimeLife.corpse.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
            GameObject bodyTwo = bodyOne.transform.GetChild(0).gameObject;
            GameObject bodyNeck = bodyTwo.transform.GetChild(2).gameObject;
            GameObject bodyHead = bodyNeck.transform.GetChild(0).transform.GetChild(1).gameObject;

            // Quitar los colliders de cada parte del cuerpo
            RemoveAllColliders(bodyOne);
            RemoveAllColliders(bodyTwo);
            RemoveAllColliders(bodyNeck);
            RemoveAllColliders(bodyHead);
        }
        catch (System.Exception) { }

        // Espera 1 segundo para que el cuerpo caiga
        yield return new WaitForSeconds(1f);

        try
        {
            // Destruye el objeto
            Destroy(chickenCorpseTimeLife.corpse);
        }
        catch (System.Exception) { }
    }

    // Método para quitar todos los colliders de un objeto y sus hijos
    private void RemoveAllColliders(GameObject obj)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }

}
