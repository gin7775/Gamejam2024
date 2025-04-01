using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSpawner : MonoBehaviour
{

    public Transform spawnPosition;
    public GameObject normalChicken;
    public bool Lock = false;

    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !Lock)
        {
            Debug.Log("Step 1 Tutorial Completed");
            Instantiate(normalChicken, spawnPosition);
            Lock = true;
        }
    }
}
