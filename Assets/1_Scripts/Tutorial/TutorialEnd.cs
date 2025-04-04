using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderOnCollision : MonoBehaviour
{
    public string sceneNameToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("End");
            SceneManager.LoadScene(sceneNameToLoad);
        }
    }
}