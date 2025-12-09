using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    public bool IsEndlessMode => endlessMode;

    [SerializeField] private string correctAnswer;
    private List<string> collectedLetters = new List<string>();
    [SerializeField]
    private QuestionOverlayController questionOverlayController;

    [Header("Scoring System")]
    [SerializeField] private int currentScore = 0;

    [Header("Game Mode Settings")]
    [SerializeField] private bool endlessMode = false;

    [Header("Endless Mode Time Bonus")]
    [SerializeField] private float easyTimeBonus = 10f;
    [SerializeField] private float normalTimeBonus = 7f;
    [SerializeField] private float hardTimeBonus = 5f;

    private float timeLimit;
    private float timeRemaining;
    private bool isTimerRunning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Reset score when starting a new game session
            ResetScore();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                if (QuestionOverlayController.Instance != null)
                {
                    QuestionOverlayController.Instance.UpdateTimerUI(timeRemaining);
                }
            }
            else
            {
                timeRemaining = 0;
                isTimerRunning = false;
                OnTimeUp();
            }

        }
    }

    // Logic để check collect đúng letter không 
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
        string currentWord = string.Join("", collectedLetters);
        if (QuestionOverlayController.Instance != null)
        {
            QuestionOverlayController.Instance.UpdateArrangedWordUI(currentWord);
        }
        CheckWordCompletion();
    }

    private void CheckWordCompletion()
    {
        string currentWord = string.Join("", collectedLetters);
        if (currentWord.Equals(correctAnswer, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("Puzzle Solved!");

            if (endlessMode)
            {
                // Add score for correct answer
                AddScoreForCorrectAnswer();

                AddTimeBonus();

                if (QuestionBankManager.Instance != null)
                {
                    // Start a new puzzle with the same difficulty
                    Question currentQuestion = QuestionBankManager.Instance.GetCurrentQuestion();
                    if (currentQuestion != null)
                    {
                        QuestionBankManager.Instance.StartPuzzle(currentQuestion.GetDifficultyLevel());
                    }
                    else
                    {
                        Debug.LogError("Cannot start new puzzle: current question is null.");
                        questionOverlayController?.ShowCompletion(); // Fallback to completion screen
                    }
                }
                else
                {
                    Debug.LogError("QuestionBankManager.Instance is not found!");
                    questionOverlayController?.ShowCompletion(); // Fallback to completion screen
                }
            }
            else
            {
                StopTimer();
                questionOverlayController?.ShowCompletion();
            }
        }
        else if (collectedLetters.Count >= correctAnswer.Length)
        {
            Debug.Log("Incorrect word. Try again.");
            // Handle incorrect attempt, e.g., reset letters
            collectedLetters.Clear();
        }
    }

    private void AddTimeBonus()
    {
        if (QuestionBankManager.Instance != null)
        {
            Question currentQuestion = QuestionBankManager.Instance.GetCurrentQuestion();
            if (currentQuestion != null)
            {
                DifficultyLevel difficulty = currentQuestion.GetDifficultyLevel();
                float bonus = difficulty switch
                {
                    DifficultyLevel.Easy => easyTimeBonus,
                    DifficultyLevel.Normal => normalTimeBonus,
                    DifficultyLevel.Hard => hardTimeBonus,
                    _ => easyTimeBonus
                };
                timeRemaining += bonus;
                Debug.Log($"Added {bonus} seconds to the timer!");
            }
        }
    }

    public void StartTimer(float duration)
    {
        if (endlessMode && isTimerRunning)
        {
            // In endless mode, if the timer is already running, don't reset it.
            // Time will be added in AddTimeBonus() instead.
            Debug.Log("Timer is already running in endless mode. No reset.");
            return;
        }

        timeLimit = duration;
        timeRemaining = duration;
        isTimerRunning = true;
        Debug.Log($"Timer started with {duration} seconds.");
    }
    public void OnTimeUp()
    {
        Debug.Log("Time's up!");

        isTimerRunning = false;

        // Save score when game ends in endless mode
        if (endlessMode)
        {
            SaveScore();
        }

        if (GameOverUI.Instance != null)
        {
            GameOverUI.Instance.ShowGameOver();
        }
        else
        {
            Debug.LogError("GameOverUI.Instance is not found in the scene!");
        }
    }
    public void StopTimer()
    {
        isTimerRunning = false;
    }
    public void SetCorrectAnswer(string answer)
    {
        correctAnswer = answer;
        collectedLetters.Clear();
        if (QuestionOverlayController.Instance != null)
        {
            QuestionOverlayController.Instance.UpdateArrangedWordUI("");
        }
    }

    private void AddScoreForCorrectAnswer()
    {
        if (QuestionBankManager.Instance != null)
        {
            Question currentQuestion = QuestionBankManager.Instance.GetCurrentQuestion();
            if (currentQuestion != null)
            {
                int points = currentQuestion.GetScoreValue();
                currentScore += points;
                Debug.Log($"Score increased by {points}. Current score: {currentScore}");

                // Force immediate UI update to ensure score is displayed correctly
                if (QuestionOverlayController.Instance != null)
                {
                    QuestionOverlayController.Instance.UpdateScoreUI(currentScore);
                    Debug.Log("UI Score update called from AddScoreForCorrectAnswer");
                }
                else
                {
                    Debug.LogError("QuestionOverlayController.Instance is null! Cannot update score UI.");
                }
            }
            else
            {
                Debug.LogError("Current question is null! Cannot calculate score.");
            }
        }
        else
        {
            Debug.LogError("QuestionBankManager.Instance is null! Cannot get current question.");
        }
    }

    private void SaveScore()
    {
        Debug.Log($"Final score to save: {currentScore}");
        // TODO: Implement actual score saving to database
        // This will be implemented later when we create the leaderboard system
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void ResetScore()
    {
        currentScore = 0;
        Debug.Log("Score reset to 0");

        // Update UI with reset score if UI controller exists
        if (QuestionOverlayController.Instance != null)
        {
            QuestionOverlayController.Instance.UpdateScoreUI(currentScore);
        }
    }
}