using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankExplosion : MonoBehaviour
{

    [Header("Explosion Settings")]
    public float explosionDelay = 3f; // Tiempo en segundos antes de ejecutar la explosión
    private bool isCodeExecuting = false;
    public List<GameObject> chickensInArea = new List<GameObject>(); // Lista de objetos detectados
    public int damageToEnemies = 1;
    public int damageToPlayer = 1;

    private void Start()
    {
        // Inicia el temporizador para ejecutar la explosión
        StartCoroutine(ExplosionTimer());
    }

    private IEnumerator ExplosionTimer()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explosion();
    }

    public void Explosion()
    {
        // Instancia partículas
        GetComponent<SpawnParticles>().SpawnBothParticles();

        if (!isCodeExecuting)
        {
            isCodeExecuting = true;

            foreach (GameObject go in chickensInArea)
            {
                if (go.CompareTag("Enemy"))
                {
                    GameManager.Instance.ChickenEnemyTakeDamage(go, damageToEnemies);
                }

                if (go.CompareTag("Player"))
                {
                    go.GetComponent<PlayerHealth>().ReceiveDamage(damageToPlayer);
                }
            }

            isCodeExecuting = false;
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            // Agregar el objeto detectado si no está ya en la lista
            if (!chickensInArea.Contains(other.gameObject))
            {
                chickensInArea.Add(other.gameObject);
                Debug.Log($"Objeto añadido: {other.gameObject.name}");
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            // Eliminar el objeto de la lista si sale del área
            if (chickensInArea.Contains(other.gameObject))
            {
                chickensInArea.Remove(other.gameObject);
                Debug.Log($"Objeto eliminado: {other.gameObject.name}");
            }
        }
            
    }

}
