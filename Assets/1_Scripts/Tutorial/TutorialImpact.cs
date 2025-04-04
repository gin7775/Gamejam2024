using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialImpact : MonoBehaviour
{
    public bool activated = false;
    public GameObject ShootTutorialManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.CompareTag("Projectile") && !activated)
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.y += 180f;
            transform.rotation = Quaternion.Euler(currentRotation);

            activated = true;

            ShootTutorialManager.GetComponent<ShootTutorial>().UpdateClick();
            //Send message
        }
    }
}
