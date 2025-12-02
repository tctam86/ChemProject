using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;

    public Image portraitImage;

    [Header("Post-Dialogue Movement")]
    public Transform waypointToMoveTo;
    public NPCDialogue dialogueAfterMove;

    private int dialogueIndex;
    private bool isTyping, isDialogActive;
    private WaypointMover mover;

    void Awake()
    {
        mover = GetComponent<WaypointMover>();
    }

    public bool CanInteract()
    {
        return !isDialogActive;
    }

    public void Interact()
    {
        if (dialogueData == null)
            return;

        if (isDialogActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogActive = true;
        dialogueIndex = 0;

        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressLinesDelay);
            NextLine();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);

        if (dialogueData.triggerMoveOnEnd && mover != null && waypointToMoveTo != null)
        {
            mover.MoveToWaypoint(waypointToMoveTo, dialogueAfterMove);
        }

        if (dialogueData.triggerGameStartOnEnd)
        {
            StartPuzzleFromDialogue();
        }
    }

    void StartPuzzleFromDialogue()
    {
        LevelStarter levelStarter = FindObjectOfType<LevelStarter>();
        if (levelStarter != null)
        {
            levelStarter.StartPuzzle();
        }
        else
        {
            Debug.LogError("LevelStarter component not found in the scene, but dialogue tried to start the puzzle.");
        }
    }
}
