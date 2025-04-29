using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighscoreTable : MonoBehaviour
{

    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highscoreEntryTransformList;

    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            for (int i = 0; i < 10; i++)
                AddHighscoreEntry(0, " ");
            jsonString = PlayerPrefs.GetString("highscoreTable");
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
        }

        // Sort
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    var tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }

        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry entry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(entry, entryContainer, highscoreEntryTransformList);
        }
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 95f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            default: rankString = rank + "TH"; break;
            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;
        }

        entryTransform.Find("posText").GetComponent<TextMeshProUGUI>().text = rankString;
        entryTransform.Find("scoreText").GetComponent<TextMeshProUGUI>().text = highscoreEntry.score.ToString();
        entryTransform.Find("nameText").GetComponent<TextMeshProUGUI>().text = highscoreEntry.name;

        transformList.Add(entryTransform);
    }

    private void AddHighscoreEntry(int score, string name)
    {
        var entry = new HighscoreEntry { score = score, name = name };
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
            highscores = new Highscores() { highscoreEntryList = new List<HighscoreEntry>() };

        highscores.highscoreEntryList.Add(entry);

        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    public void CheckAndAddHighscore(int score, string name)
    {
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
            highscores = new Highscores() { highscoreEntryList = new List<HighscoreEntry>() };

        if (highscores.highscoreEntryList.Count < 10 || score > highscores.highscoreEntryList[highscores.highscoreEntryList.Count - 1].score)
        {
            highscores.highscoreEntryList.Add(new HighscoreEntry { score = score, name = name });
            highscores.highscoreEntryList.Sort((x, y) => y.score.CompareTo(x.score));
            if (highscores.highscoreEntryList.Count > 10)
                highscores.highscoreEntryList.RemoveAt(10);

            string json = JsonUtility.ToJson(highscores);
            PlayerPrefs.SetString("highscoreTable", json);
            PlayerPrefs.Save();
        }
    }

    public bool CheckIfTop10(int score)
    {
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
            highscores = new Highscores() { highscoreEntryList = new List<HighscoreEntry>() };

        return highscores.highscoreEntryList.Count < 10 || score > highscores.highscoreEntryList[^1].score;
    }

    public void RefreshHighscoreTable()
    {
        foreach (Transform entry in highscoreEntryTransformList)
        {
            Destroy(entry.gameObject);
        }
        highscoreEntryTransformList.Clear();

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
            highscores = new Highscores() { highscoreEntryList = new List<HighscoreEntry>() };

        highscores.highscoreEntryList.Sort((x, y) => y.score.CompareTo(x.score));

        foreach (HighscoreEntry entry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(entry, entryContainer, highscoreEntryTransformList);
        }
    }

    public void ResetHighscoreTable()
    {
        PlayerPrefs.DeleteKey("highscoreTable");
        PlayerPrefs.Save();

        for (int i = 0; i < 10; i++)
            AddHighscoreEntry(0, " ");
    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    [System.Serializable]
    private class HighscoreEntry
    {
        public int score;
        public string name;
    }
}
