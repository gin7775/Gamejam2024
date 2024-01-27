using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;


public class PauseMenu : MonoBehaviour
{

    public bool paused = false;
    public GameObject pausemenu;

    //Music Variables
    public AudioMixer myMixer;
    public Slider musicSlider;
    public Slider SFXSlider;


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

    //Music Manager

    public void SetMusicVolume()
    {
        Debug.Log("Music modified");
        float volume = musicSlider.value;
        Debug.Log(musicSlider.value);
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20); 
    }
    public void SetSFXVolume()
    {
        
        float volume = SFXSlider.value;
        myMixer.SetFloat("FX", Mathf.Log10(volume) * 20); 
    }
    

}
