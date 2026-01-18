using UnityEngine;
using NativeWebSocket;
using System.Collections.Generic;

[System.Serializable]
public class VoiceCommand
{
    public string type;
    public string data;


}
public class VoskWebSocketClient : MonoBehaviour
{
    [Tooltip("Character")]
    public MiniPlatformerController playerController;
    [Tooltip("IP address of Python server")]
    public string serverAddress = "localhost";
    [Tooltip("Port of Python server")]
    public int serverPort = 8765;
    [Tooltip("Key for push-to-talk")]
    public KeyCode pushToTalkKey = KeyCode.V;

    private WebSocket websocket;
    private readonly Queue<string> commandQueue = new Queue<string>();
    private bool isPushToTalkActive = false;


    async void Start()
    {
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<MiniPlatformerController>();
            if (playerController == null)
            {
                Debug.LogError("Error: Cannot find the script 'MiniPlatformerController!'");
                this.enabled = false;
                return;
            }
        }
        string serverUrl = $"ws://{serverAddress}:{serverPort}";
        websocket = new WebSocket(serverUrl);

        websocket.OnOpen += () =>
        {
            Debug.Log($"Connected to: {serverUrl}");
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError($"WebSocket Error: {e}");
        };
        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket Connection Closed");
        };
        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            lock (commandQueue)
            {
                commandQueue.Enqueue(message);
            }
        };

        Debug.Log($"Connecting to {serverUrl}...");
        await websocket.Connect();
    }
    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            websocket.DispatchMessageQueue();
        }
#endif

        // Handle push-to-talk
        if (Input.GetKeyDown(pushToTalkKey))
        {
            StartPushToTalk();
        }
        else if (Input.GetKeyUp(pushToTalkKey))
        {
            StopPushToTalk();
        }

        while (commandQueue.Count > 0)
        {
            string message;
            lock (commandQueue)
            {
                message = commandQueue.Dequeue();
            }
            ProcessCommand(message);
        }
    }

    private void StartPushToTalk()
    {
        isPushToTalkActive = true;
        SendControlMessage("start_listening");
        Debug.Log("Push-to-talk: Start listening");
    }

    private void StopPushToTalk()
    {
        isPushToTalkActive = false;
        SendControlMessage("stop_listening");
        Debug.Log("Push-to-talk: Stop listening");
    }

    private async void SendControlMessage(string action)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            string jsonMessage = JsonUtility.ToJson(new VoiceCommand { type = "control", data = action });
            await websocket.SendText(jsonMessage);
        }
    }

    private void ProcessCommand(string jsonMessage)
    {
        try
        {
            VoiceCommand cmd = JsonUtility.FromJson<VoiceCommand>(jsonMessage);
            if (cmd != null && cmd.type == "command" && playerController != null)
            {
                if (isPushToTalkActive)
                {
                    Debug.Log($"Receive: {cmd.data}");
                    playerController.ExecuteVoiceCommand(cmd.data);
                }
                else
                {
                    Debug.Log($"Ignored (V not held): {cmd.data}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Cannot parse  JSON command '{jsonMessage}': {e.Message}");
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            await websocket.Close();
        }
    }

}
