using UnityEngine;

public class VoiceMovementController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private VoiceCommandParser commandParser;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        commandParser = FindObjectOfType<VoiceCommandParser>();

        if (commandParser != null)
        {
            commandParser.OnMovementCommand += HandleMovementCommand;
        }

        if (VoiceRecognitionManager.Instance != null)
        {
            VoiceRecognitionManager.Instance.StartListening();
        }
    }

    void HandleMovementCommand(Vector2 direction)
    {
        if (playerMovement != null)
        {
            playerMovement.MoveByVoice(direction);
        }
    }

    void OnDestroy()
    {
        if (commandParser != null)
        {
            commandParser.OnMovementCommand -= HandleMovementCommand;
        }
    }
}
