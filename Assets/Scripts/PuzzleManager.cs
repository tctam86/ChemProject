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
    //Logic để check collect đúng letter không 
    public bool TryCollectLetter(string letter)
    {
        if (string.IsNullOrEmpty(correctAnswer))
        {
            Debug.Log("No correct answer set.");
            return false;
        }
        int index = collectedLetters.Count;

        if (index >= correctAnswer.Length)
        {
            Debug.Log("Puzzle already solved or input overflow.");
            return false;
        }
        char expected = char.ToUpperInvariant(correctAnswer[index]);
        char picked = char.ToUpperInvariant(string.IsNullOrEmpty(letter) ? '\0' : letter[0]);
        if (picked == expected)
        {
            Debug.Log($"✅ Correct letter: {letter}");
            return true;
        }
        return false;
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