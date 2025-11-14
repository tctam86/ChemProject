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
    [Tooltip("Drag Character's GameObject here")]
    public MiniPlatformerController playerController;
    [Tooltip("IP address of Python server")]
    public string serverAddress = "localhost";
    [Tooltip("Port of Python server")]
    public int serverPort = 8765;

    private WebSocket websocket;
    private readonly Queue<string> commandQueue = new Queue<string>();


    async void Start()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<MiniPlatformerController>();
            if (playerController == null)
            {
                Debug.LogError("LỖI: Không tìm thấy script 'MiniPlatformerController' trong Scene!");
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

    private void ProcessCommand(string jsonMessage)
    {
        try
        {
            VoiceCommand cmd = JsonUtility.FromJson<VoiceCommand>(jsonMessage);
            if (cmd != null && cmd.type == "command" && playerController != null)
            {
                Debug.Log($"Nhận được lệnh: {cmd.data}");
                playerController.ExecuteVoiceCommand(cmd.data);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Không thể parse lệnh JSON '{jsonMessage}': {e.Message}");
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
