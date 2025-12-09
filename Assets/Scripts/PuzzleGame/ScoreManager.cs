using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ScoreEntry
{
    public int score;
    public DateTime date;
    public string difficulty;

    public ScoreEntry(int score, string difficulty)
    {
        this.score = score;
        this.date = DateTime.Now;
        this.difficulty = difficulty;
    }
}

[System.Serializable]
public class ScoreData
{
    public List<ScoreEntry> scores = new List<ScoreEntry>();
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private string saveFilePath;
    private ScoreData scoreData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        saveFilePath = Path.Combine(Application.persistentDataPath, "Scores.json");
        LoadScores();
    }

    private void LoadScores()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string jsonContent = File.ReadAllText(saveFilePath);
                scoreData = JsonUtility.FromJson<ScoreData>(jsonContent);
                if (scoreData == null)
                {
                    scoreData = new ScoreData();
                }
                Debug.Log($"Loaded {scoreData.scores.Count} scores from file");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading scores: {e.Message}");
                scoreData = new ScoreData();
            }
        }
        else
        {
            scoreData = new ScoreData();
            Debug.Log("No existing score file found, creating new one");
        }
    }

    private void SaveScores()
    {
        try
        {
            string jsonContent = JsonUtility.ToJson(scoreData, true);
            File.WriteAllText(saveFilePath, jsonContent);
            Debug.Log($"Saved {scoreData.scores.Count} scores to file");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving scores: {e.Message}");
        }
    }

    public void SaveScore(int score, string difficulty)
    {
        ScoreEntry newEntry = new ScoreEntry(score, difficulty);
        scoreData.scores.Add(newEntry);

        // Keep only the last 100 scores to prevent file from getting too large
        if (scoreData.scores.Count > 100)
        {
            scoreData.scores.Sort((a, b) => b.score.CompareTo(a.score));
            scoreData.scores.RemoveRange(100, scoreData.scores.Count - 100);
        }

        SaveScores();
        Debug.Log($"Saved new score: {score} points with difficulty: {difficulty}");
    }

    public List<ScoreEntry> GetTopScores(int count)
    {
        List<ScoreEntry> sortedScores = new List<ScoreEntry>(scoreData.scores);
        sortedScores.Sort((a, b) => b.score.CompareTo(a.score));

        if (sortedScores.Count > count)
        {
            sortedScores.RemoveRange(count, sortedScores.Count - count);
        }

        return sortedScores;
    }

    public List<ScoreEntry> GetAllScores()
    {
        return new List<ScoreEntry>(scoreData.scores);
    }
}