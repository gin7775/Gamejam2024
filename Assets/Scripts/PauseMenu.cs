using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public bool paused = false;
    public GameObject pausemenu;

    public void Start()
    {
        pausemenu.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape") && paused == false)
        {
            pause();
        }

        if (Input.GetKey("r") && paused)
        {
            exit_pause_menu();

        }
    }

    public void exit_pause_menu()
    {
        
        Time.timeScale = 1;
        paused = false;
        pausemenu.SetActive(false);
    }

    public void pause()
    {
        paused = true;
        pausemenu.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Back_to_Main_Menu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MenuPrinciapl");
    }

}
