using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionOverlayController : MonoBehaviour
{
    public static QuestionOverlayController Instance { get; private set; }

    [Header("Overlay References")]
    [SerializeField] private CanvasGroup overlayGroup;
    [SerializeField] private GameObject panelFrame;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private GameObject choicesContainer;
    [SerializeField] private TMP_Text timerText;

    [Header("Completion UI")]
    [SerializeField] private GameObject completionGroup;
    [SerializeField] private TMP_Text congratsText;
    [SerializeField] private Button continueButton;

    [Header("Animation")]
    [SerializeField] private Animator animator;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        HideImmediate();
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(Hide);
        }
    }
    public void ShowQuestion(Question question)
    {
        Debug.Log("ShowQuestion called with question: " + (question?.questionText ?? "null"));

        if (question == null)
        {
            Debug.LogError("Question is null!");
            return;
        }

        gameObject.SetActive(true);
        Debug.Log("GameObject set to active");

        if (questionText != null)
        {
            questionText.gameObject.SetActive(true);
            questionText.text = question.questionText;
            Debug.Log("Question text set to: " + question.questionText);
        }
        else
        {
            Debug.LogError("QuestionText is null! Check the Inspector assignment.");
        }

        if (difficultyText != null)
        {
            difficultyText.gameObject.SetActive(true);
            difficultyText.text = $"Difficulty: {question.difficulty}";
            Debug.Log("Difficulty text set to: " + question.difficulty);
        }
        else
        {
            Debug.LogError("DifficultyText is null! Check the Inspector assignment.");
        }

        if (choicesContainer != null) { choicesContainer.SetActive(true); }
        if (completionGroup != null)
        {
            completionGroup.SetActive(false);
            overlayGroup.alpha = 1f;
            overlayGroup.interactable = true;
            overlayGroup.blocksRaycasts = true;
        }

        if (animator != null)
        {
            animator.SetTrigger("ShowQuestion");

            Debug.Log("Animator trigger ShowQuestion set");
        }
        else
        {
            Debug.LogWarning("Animator is null");
        }
    }

    public void ShowCompletion()
    {

        if (choicesContainer != null) choicesContainer.SetActive(false);
        if (difficultyText != null) difficultyText.gameObject.SetActive(false);
        if (choicesContainer != null) choicesContainer.SetActive(false);
        if (completionGroup != null)
        {
            completionGroup.SetActive(true);
            if (congratsText != null)
            {
                congratsText.text = "Congratulations! You completed the puzzle!";
            }
            else
            {
                Debug.LogError("CongratsText is not assigned in the Inspector!");
            }
        }

        if (animator != null) animator.SetTrigger("ShowComplete");
    }
    public void Hide()
    {
        overlayGroup.interactable = false;
        overlayGroup.blocksRaycasts = false;
        if (animator != null)
        {
            animator.SetTrigger("Hide");
        }
        else
        {
            overlayGroup.alpha = 0f;
            gameObject.SetActive(false);

        }
    }
    private void HideImmediate()
    {
        if (overlayGroup != null)
        {
            overlayGroup.alpha = 0f;
            overlayGroup.interactable = false;
            overlayGroup.blocksRaycasts = false;
        }
        if (completionGroup != null) completionGroup.SetActive(false);
    }

    public void UpdateTimerUI(float time)
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.color = (time <= 10f) ? Color.red : Color.white;
    }

}

