using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextUiScript : MonoBehaviour
{
    public string textNumWave =     "00 / 00";
    public string textNumEnemies =  "00 / 00";
    public string textTimer =       "00 / 00";
    public GameObject textMeshNumWave;
    public GameObject textMeshNumEnemies;
    public GameObject textMeshTimer;

    private float tiempoPasado = 0f;
    private bool estaCorriendo = false;

    private void Start()
    {
        IniciarCronometro();
        tiempoPasado = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (estaCorriendo)
        {
            tiempoPasado += Time.deltaTime;
            textMeshTimer.GetComponent<TextMeshProUGUI>().text = FormatearTiempo(tiempoPasado);
        }

        textMeshNumWave.GetComponent<TextMeshProUGUI>().text = "0" + GameManager.Instance.waveCurrent + " / 0" + GameManager.Instance.waveNumber;
        textMeshNumEnemies.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.enemyCount + " / " + GameManager.Instance.enemyNumber;
    }

    void ActualizarTiempoUI()
    {
        // Formatear el tiempo en formato HH:mm:ss
        string tiempoFormateado = FormatearTiempo(tiempoPasado);

        // En este ejemplo, simplemente imprimo el tiempo en la consola.
        // Puedes usar este método para actualizar un texto en la interfaz de usuario (UI) u otros elementos visuales.
        //Debug.Log("Tiempo transcurrido: " + tiempoFormateado);
    }

    public string FormatearTiempo(float tiempo)
    {
        // Formatear el tiempo en formato HH:mm:ss
        int horas = (int) (tiempo / 3600);
        int minutos = (int) ((tiempo % 3600) / 60);
        int segundos = (int) (tiempo % 60);

        return string.Format("{0:D2}:{1:D2}:{2:D2}", horas, minutos, segundos);
    }

    public void IniciarCronometro()
    {
        estaCorriendo = true;
    }

    public void DetenerCronometro()
    {
        estaCorriendo = false;
        ActualizarTiempoUI(); // Puedes llamar a esto para asegurarte de que el tiempo final se muestra correctamente.
    }

    public void ReiniciarCronometro()
    {
        tiempoPasado = 0f;
        ActualizarTiempoUI();
    }
}
