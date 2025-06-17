using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
 

public class SceneChangers : MonoBehaviour
{
    public string 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
