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
        textMeshNumWave.GetComponent<TextMeshProUGUI>().text = $"{GameManager.Instance.level.ToString("D2")}  /  {GameManager.Instance.waveCurrent.ToString("D3")}";
        textMeshNumEnemies.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.enemyCount.ToString("D3");// + " / " + GameManager.Instance.totalWaveChicken.ToString("D3");
        textMeshScore.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.score.ToString("D5");
    }

}
