using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting.Antlr3.Runtime;

public class GameManager2 : MonoBehaviour
{
    //public static GameManager Instance { get; private set; }

    [Header("UI Control")]
    public GameObject pauseMenu;
    public GameObject highscorePanel;
    public GameObject restartButton;
    public GameObject nameInput;

    [Header("Music Control")]
    [SerializeField] private MusicManager musicManager;
    public AudioMixer audioMixer;
    public Slider musicSlider, sfxSlider;

    [Header("Pause Menu")]
    public bool isPaused = false;
    public GameObject firstGameObjectMenu;

    private void Awake()
    {
        //if (Instance != null && Instance != this)
        //    Destroy(this);
        //else
        //    Instance = this;
    }

    public void Pause()
    {
        if (isPaused)
            Resume();
        else
        {
            isPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            EventSystem.current.SetSelectedGameObject(firstGameObjectMenu);
        }
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ExitPauseMenu()
    {
        Time.timeScale = 1;
        isPaused = false;
        pauseMenu.SetActive(false);
        highscorePanel.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Back_to_Main_Menu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("0_MenuPrincipal");
    }

    public void SetMusicVolume()
    {
        audioMixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
    }

    public void SetSFXVolume()
    {
        audioMixer.SetFloat("FX", Mathf.Log10(sfxSlider.value) * 20);
    }

    public void FrameFreeze(float duration)
    {
        StartCoroutine(FrameFreezeCoroutine(duration));
    }

    private IEnumerator FrameFreezeCoroutine(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

}