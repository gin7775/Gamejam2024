using TMPro;
using UnityEngine;

public class TextUiScript : MonoBehaviour
{
    public GameObject textMeshNumWave;
    public GameObject textMeshNumEnemies;
    public GameObject textMeshScore;

    void Update()
    {
        textMeshNumWave.GetComponent<TextMeshProUGUI>().text = $"{GameManager.Instance.level.ToString("D2")}  /  {GameManager.Instance.waveCurrent.ToString("D3")}";
        textMeshNumEnemies.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.enemyCount.ToString("D3");// + " / " + GameManager.Instance.totalWaveChicken.ToString("D3");
        textMeshScore.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.score.ToString();
    }

}
