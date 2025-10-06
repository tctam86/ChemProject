using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [SerializeField] private string correctAnswer;
    private List<string> collectedLetters = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnLetterCollected(string letter)
    {
        collectedLetters.Add(letter);
        CheckWordCompletion();
    }

    private void CheckWordCompletion()
    {
        string currentWord = string.Join("", collectedLetters);
        if (currentWord.Equals(correctAnswer, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("Puzzle Solved!");
            // Trigger success events, e.g., open gate
        }
        else if (collectedLetters.Count >= correctAnswer.Length)
        {
            Debug.Log("Incorrect word. Try again.");
            // Handle incorrect attempt, e.g., reset letters
            collectedLetters.Clear();
        }
    }

    public void SetCorrectAnswer(string answer)
    {
        correctAnswer = answer;
        collectedLetters.Clear();
    }
}