using UnityEngine;
using System;
using System.Collections.Generic;

public class VoiceCommandParser : MonoBehaviour
{
    private Dictionary<string, Vector2> commandMappings = new Dictionary<string, Vector2>();

    public event Action<Vector2> OnMovementCommand;
    public event Action<string> OnCommandRecognized;

    void Start()
    {
        InitializeCommands();

        if (VoiceRecognitionManager.Instance != null)
        {
            VoiceRecognitionManager.Instance.OnSpeechRecognized += HandleSpeechRecognized;
        }
    }

    void InitializeCommands()
    {
        // Movement commands
        commandMappings["left"] = Vector2.left;
        commandMappings["right"] = Vector2.right;
        commandMappings["up"] = Vector2.up;
        commandMappings["down"] = Vector2.down;

        // Alternative commands
        commandMappings["go left"] = Vector2.left;
        commandMappings["go right"] = Vector2.right;
        commandMappings["go up"] = Vector2.up;
        commandMappings["go down"] = Vector2.down;

        commandMappings["move left"] = Vector2.left;
        commandMappings["move right"] = Vector2.right;
        commandMappings["move up"] = Vector2.up;
        commandMappings["move down"] = Vector2.down;
    }

    void HandleSpeechRecognized(string recognizedText)
    {
        Debug.Log($"Recognized: {recognizedText}");

        string cleanText = recognizedText.ToLower().Trim();

        if (commandMappings.TryGetValue(cleanText, out Vector2 movement))
        {
            OnMovementCommand?.Invoke(movement);
            OnCommandRecognized?.Invoke(cleanText);
            return;
        }

        foreach (var kvp in commandMappings)
        {
            if (cleanText.Contains(kvp.Key))
            {
                OnMovementCommand?.Invoke(kvp.Value);
                OnCommandRecognized?.Invoke(kvp.Key);
                return;
            }
        }
    }

    void OnDestroy()
    {
        if (VoiceRecognitionManager.Instance != null)
        {
            VoiceRecognitionManager.Instance.OnSpeechRecognized -= HandleSpeechRecognized;
        }
    }
}
