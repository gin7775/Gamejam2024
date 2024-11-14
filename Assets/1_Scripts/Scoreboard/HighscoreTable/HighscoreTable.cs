using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreTable : MonoBehaviour {

    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highscoreEntryTransformList;

    private void Awake() {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null) {
            // There's no stored table, initialize
            //Debug.Log("Initializing table with default values...");
            AddHighscoreEntry(0, " ");
            AddHighscoreEntry(0, " ");
            AddHighscoreEntry(0, " ");
            AddHighscoreEntry(0, " ");
            AddHighscoreEntry(0, " ");
            AddHighscoreEntry(0, " ");
            AddHighscoreEntry(0, " ");
            AddHighscoreEntry(0, " ");
            AddHighscoreEntry(0, " ");
            AddHighscoreEntry(0, " ");
            // Reload
            jsonString = PlayerPrefs.GetString("highscoreTable");
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
        }

        // Sort entry list by Score
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++) {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++) {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score) {
                    // Swap
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }

        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList) {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList) {
        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank) {
        default:
            rankString = rank + "TH"; break;

        case 1: rankString = "1ST"; break;
        case 2: rankString = "2ND"; break;
        case 3: rankString = "3RD"; break;
        }

        entryTransform.Find("posText").GetComponent<Text>().text = rankString;

        int score = highscoreEntry.score;

        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        string name = highscoreEntry.name;
        entryTransform.Find("nameText").GetComponent<Text>().text = name;

        // Set background visible odds and evens, easier to read
        //entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);
        
        //// Highlight First
        //if (rank == 1) {
        //    entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
        //    entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
        //    entryTransform.Find("nameText").GetComponent<Text>().color = Color.green;
        //}

        transformList.Add(entryTransform);
    }

    private void AddHighscoreEntry(int score, string name) {
        // Create HighscoreEntry
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name };
        
        // Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null) {
            // There's no stored table, initialize
            highscores = new Highscores() {
                highscoreEntryList = new List<HighscoreEntry>()
            };
        }

        // Add new entry to Highscores
        highscores.highscoreEntryList.Add(highscoreEntry);

        // Save updated Highscores
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    public void CheckAndAddHighscore(int score, string name)
    {
        // Cargar la tabla de puntuaciones
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            highscores = new Highscores()
            {
                highscoreEntryList = new List<HighscoreEntry>()
            };
        }

        // Verificar si la nueva puntuación debería estar en el top 10
        if (highscores.highscoreEntryList.Count < 10 || score > highscores.highscoreEntryList[highscores.highscoreEntryList.Count - 1].score)
        {
            // Añadir la nueva puntuación
            HighscoreEntry newEntry = new HighscoreEntry { score = score, name = name };
            highscores.highscoreEntryList.Add(newEntry);

            // Ordenar la lista de mayor a menor
            highscores.highscoreEntryList.Sort((x, y) => y.score.CompareTo(x.score));

            // Mantener solo las 10 mejores puntuaciones
            if (highscores.highscoreEntryList.Count > 10)
            {
                highscores.highscoreEntryList.RemoveAt(highscores.highscoreEntryList.Count - 1);
            }

            // Guardar la tabla actualizada
            string json = JsonUtility.ToJson(highscores);
            PlayerPrefs.SetString("highscoreTable", json);
            PlayerPrefs.Save();

            Debug.Log($"Nueva puntuación añadida: {name} - {score}");
        }
        else
        {
            Debug.Log("La puntuación no es lo suficientemente alta para entrar en el top 10.");
        }
    }

    public void ResetHighscoreTable()
    {
        // Eliminar los datos guardados en PlayerPrefs para la tabla de puntuaciones
        PlayerPrefs.DeleteKey("highscoreTable");
        PlayerPrefs.Save();

        AddHighscoreEntry(0, " ");
        AddHighscoreEntry(0, " ");
        AddHighscoreEntry(0, " ");
        AddHighscoreEntry(0, " ");
        AddHighscoreEntry(0, " ");
        AddHighscoreEntry(0, " ");
        AddHighscoreEntry(0, " ");
        AddHighscoreEntry(0, " ");
        AddHighscoreEntry(0, " ");
        AddHighscoreEntry(0, " ");
    }


    private class Highscores {
        public List<HighscoreEntry> highscoreEntryList;
    }

    /*
     * Represents a single High score entry
     * */
    [System.Serializable] 
    private class HighscoreEntry {
        public int score;
        public string name;
    }

}
