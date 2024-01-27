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
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20); PlayerPrefs.SetFloat("musicVolume", volume);
    }
    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20); PlayerPrefs.SetFloat("SFXVolume", volume);
    }
    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("musi Volume");
        SetMusicVolume();
    }

}
