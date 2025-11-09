using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionOverlayController : MonoBehaviour
{
    [Header("Overlay References")]
    [SerializeField] private CanvasGroup overlayGroup;
    [SerializeField] private GameObject panelFrame;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private GameObject choicesContainer;

    [Header("Completion UI")]
    [SerializeField] private GameObject completionGroup;
    [SerializeField] private TMP_Text congratsText;
    [SerializeField] private Button continueButton;

    [Header("Animation")]
    [SerializeField] private Animator animator;



    private void Start()
    {
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
            questionText.text = question.questionText;
            Debug.Log("Question text set to: " + question.questionText);
        }
        else
        {
            Debug.LogError("QuestionText is null! Check the Inspector assignment.");
        }

        if (difficultyText != null)
        {
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
}

