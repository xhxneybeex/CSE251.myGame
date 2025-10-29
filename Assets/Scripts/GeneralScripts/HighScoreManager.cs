using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HighScoreEntry
{
    public int score;
    public string date; // when score was saved
}

[Serializable]
public class HighScoreData
{
    public List<HighScoreEntry> entries = new List<HighScoreEntry>(); // list of scores
}

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("How many top scores to keep.")]
    public int maxEntries = 10;

    private const string KEY = "HighScores"; // save key
    private HighScoreData data = new HighScoreData(); // local data

    void Awake()
    {
        // make sure only one exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load(); // load saved scores
    }

    public void AddScore(int score)
    {
        if (score < 0) score = 0;

        // create new score entry
        var entry = new HighScoreEntry
        {
            score = score,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'")
        };

        data.entries.Add(entry);
        // sort from highest to lowest
        data.entries.Sort((a, b) => b.score.CompareTo(a.score));
        // trim list if too long
        if (data.entries.Count > maxEntries)
            data.entries.RemoveRange(maxEntries, data.entries.Count - maxEntries);

        Save(); // save updated list
    }

    public IReadOnlyList<HighScoreEntry> GetScores()
    {
        return data.entries; // return all scores
    }

    public void ClearAll()
    {
        // wipe everything
        data.entries.Clear();
        PlayerPrefs.DeleteKey(KEY);
        PlayerPrefs.Save();
    }

    private void Save()
    {
        // turn data into JSON and save , JSON good for saving data, it is javascript object notation.
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        // try to load from PlayerPrefs
        if (PlayerPrefs.HasKey(KEY))
        {
            string json = PlayerPrefs.GetString(KEY);
            try
            {
                data = JsonUtility.FromJson<HighScoreData>(json) ?? new HighScoreData();
            }
            catch
            {
                data = new HighScoreData(); // reset if broken, if JSON corrupted
            }
        }
        else
        {
            data = new HighScoreData(); // start fresh
        }
    }
}
