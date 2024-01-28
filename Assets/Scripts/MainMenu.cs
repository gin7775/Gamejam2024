using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SceneGame(int sceneToChange)
    {

        switch (sceneToChange)
        {
            case 1:
                SceneManager.LoadScene("1_MenuPrincipal");
                break;
            case 2:
                SceneManager.LoadScene("Opciones");
                break;
            case 3:
                SceneManager.LoadScene("Credits");
                break;
            case 4:
                CargarCorutinaNivel();
                break;
            case 5:
                Application.Quit();
                break;

        }

    }

    public void CargarCorutinaNivel( )
    {
        StartCoroutine(CorutinaCargarNivel());
    }

    IEnumerator CorutinaCargarNivel()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("UnaiScene (BLOCKING)");
    }

    public void CargarNivel(string nombreNivel)
    {
        SceneManager.LoadScene(nombreNivel);
    }

}

