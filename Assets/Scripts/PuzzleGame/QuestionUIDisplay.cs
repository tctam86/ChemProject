using UnityEngine;
using TMPro;


public class QuestionUIDisplay : MonoBehaviour
{
    public static QuestionUIDisplay Instance { get; private set; }
    [Header("UI Elements")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private TMP_Text timerText;

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
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
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
    public void UpdateTimerUI(float time)
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);


        if (time <= 10f)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }
    }
}
