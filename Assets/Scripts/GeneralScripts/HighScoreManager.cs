using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HighScoreEntry
{
    public int score;
    public string date; // optional: ISO string for when it happened
}

[Serializable]
public class HighScoreData
{
    public List<HighScoreEntry> entries = new List<HighScoreEntry>();
}

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("How many top scores to keep.")]
    public int maxEntries = 10;

    private const string KEY = "HighScores";
    private HighScoreData data = new HighScoreData();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    public void AddScore(int score)
    {
        if (score < 0) score = 0;

        var entry = new HighScoreEntry
        {
            score = score,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'")
        };

        data.entries.Add(entry);
        // sort high → low and cap list
        data.entries.Sort((a, b) => b.score.CompareTo(a.score));
        if (data.entries.Count > maxEntries)
            data.entries.RemoveRange(maxEntries, data.entries.Count - maxEntries);

        Save();
    }

    public IReadOnlyList<HighScoreEntry> GetScores()
    {
        return data.entries;
    }

    public void ClearAll()
    {
        data.entries.Clear();
        PlayerPrefs.DeleteKey(KEY);
        PlayerPrefs.Save();
    }

    private void Save()
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        if (PlayerPrefs.HasKey(KEY))
        {
            string json = PlayerPrefs.GetString(KEY);
            try
            {
                data = JsonUtility.FromJson<HighScoreData>(json) ?? new HighScoreData();
            }
            catch
            {
                data = new HighScoreData();
            }
        }
        else
        {
            data = new HighScoreData();
        }
    }
}
