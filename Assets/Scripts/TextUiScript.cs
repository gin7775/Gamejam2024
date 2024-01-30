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
    public GameObject textMeshScore;

    void Update()
    {
        if (GameManager.Instance
            .waveCurrent >= 4)
        {
            textMeshNumWave.GetComponent<TextMeshProUGUI>().text = 999 + " / 0" + GameManager.Instance.waveNumber;
        }
        else
        {
            textMeshNumWave.GetComponent<TextMeshProUGUI>().text = "0" + GameManager.Instance.waveCurrent + " / 0" + GameManager.Instance.waveNumber;
        }
        textMeshNumEnemies.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.enemyCount + " / " + (GameManager.Instance.enemyNumber + GameManager.Instance.enemyInitial * GameManager.Instance.waveCurrent);
        textMeshScore.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.score + "";
    }

}
