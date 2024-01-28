using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public AudioSource sonidoClic;
    public AudioSource musica;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SceneGame(int sceneToChange)
    {

        switch (sceneToChange)
        {
            case 1:
                CargarCorutinaNivel();
                break;
            case 2:
                SceneManager.LoadScene("3_Opciones");
                break;
            case 3:
                SceneManager.LoadScene("2_Credits");
                break;
            case 4:
                CargarCorutinaNivel();
                break;
            case 5:
                Application.Quit();
                break;

        }

    }

    public void CargarCorutinaNivel()
    {
        StartCoroutine(CorutinaCargarNivel());
    }

    IEnumerator CorutinaCargarNivel()
    {
        sonidoClic.Play();
        yield return new WaitForSeconds(1f);
        musica.Stop();
        SceneManager.LoadScene("1_MainGame");
    }

    public void CargarNivel(string nombreNivel)
    {
        SceneManager.LoadScene(nombreNivel);
    }

}

