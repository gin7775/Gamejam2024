using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Retry : MonoBehaviour
{

    public string RetryScene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reload()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Back_to_Main_Menu()
    {
        SceneManager.LoadScene("0_MenuPrincipal");
    }
}
