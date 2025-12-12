using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;

    public Image portraitImage;

    [Header("Audio")]
    public AudioClip typingSound;
    private AudioSource audioSource;

    [Header("Post-Dialogue Movement")]
    public Transform waypointToMoveTo;
    public NPCDialogue dialogueAfterMove;

    private int dialogueIndex;
    private bool isTyping, isDialogActive;
    private WaypointMover mover;
    private bool isPermanentlyDisabled = false;

    void Awake()
    {
        mover = GetComponent<WaypointMover>();
        audioSource = GetComponent<AudioSource>();
    }

    public bool CanInteract()
    {
        return !isDialogActive && !isPermanentlyDisabled;
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
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
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

        if (typingSound != null)
        {
            audioSource.clip = typingSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(dialogueData.typingSpeed);
        }

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        isTyping = false;

        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSecondsRealtime(dialogueData.autoProgressLinesDelay);
            NextLine();
        }
    }

    public void EndDialogue()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
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

        if (dialogueData.disableInteractionOnEnd)
        {
            isPermanentlyDisabled = true;
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
