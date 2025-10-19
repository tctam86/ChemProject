using UnityEngine;
using TMPro;


public class QuestionUIDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private GameObject questionPanel;

    [Header("Display Settings")]

    [SerializeField] private float displayDuration = 5f;
    [SerializeField] private bool autoHide = true;

    private float displayTimer;

    void Start()
    {
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
        }
    }


    void Update()
    {
        if (autoHide && questionPanel != null && questionPanel.activeSelf)
        {
            displayTimer -= Time.deltaTime;
            if (displayTimer <= 0)
            {
                HideQuestion();
            }
        }
    }

    public void DisplayQuestion(Question question)
    {
        Debug.Log($"Question: {question.questionText}");
        Debug.Log($"Answer: {question.answer}");
        Debug.Log($"Difficulty: {question.difficulty}");
        Debug.Log($"Answer Length: {question.answer.Length} letters");
        if (questionText != null)
        {
            questionText.text = question.questionText;
        }

        if (difficultyText != null)
        {
            difficultyText.text = $"Difficulty: {question.difficulty}";

            // Fixed: Use GetDifficultyLevel() to get enum
            difficultyText.color = question.GetDifficultyLevel() switch
            {
                DifficultyLevel.Easy => Color.green,
                DifficultyLevel.Normal => Color.yellow,
                DifficultyLevel.Hard => Color.red,
                _ => Color.white
            };
        }

        if (questionPanel != null)
        {
            questionPanel.SetActive(true);
            displayTimer = displayDuration;
        }
    }

    public void HideQuestion()
    {
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
        }
    }
    public void ShowQuestion()
    {
        if (questionPanel != null)
        {
            questionPanel.SetActive(true);
        }
    }
}
