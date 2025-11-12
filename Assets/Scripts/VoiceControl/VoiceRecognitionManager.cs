using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class VoiceRecognitionManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void _initSpeechRecognizer();

    [DllImport("__Internal")]
    private static extern bool _requestSpeechPermissions();

    [DllImport("__Internal")]
    private static extern void _startSpeechRecognition(IntPtr callback);

    [DllImport("__Internal")]
    private static extern void _stopSpeechRecognition();

    private delegate void SpeechRecognitionCallbackDelegate(string recognizedText);
    private static SpeechRecognitionCallbackDelegate speechRecognitionDelegate;

    public static VoiceRecognitionManager Instance { get; private set; }

    public bool IsListening { get; private set; }
    public event Action<string> OnSpeechRecognized;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSpeechRecognizer();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeSpeechRecognizer()
    {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        _initSpeechRecognizer();
        speechRecognitionDelegate = new SpeechRecognitionCallbackDelegate(OnSpeechRecognizedCallback);
#endif
    }

    [AOT.MonoPInvokeCallback(typeof(SpeechRecognitionCallbackDelegate))]
    private static void OnSpeechRecognizedCallback(string recognizedText)
    {
        if (Instance != null)
        {
            Instance.OnSpeechRecognized?.Invoke(recognizedText.ToLower());
        }
    }

    public void StartListening()
    {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        if (!IsListening)
        {
            if (_requestSpeechPermissions())
            {
                _startSpeechRecognition(Marshal.GetFunctionPointerForDelegate(speechRecognitionDelegate));
                IsListening = true;
                Debug.Log("Speech recognition started");
            }
            else
            {
                Debug.LogError("Speech recognition permissions denied");
            }
        }
#endif
    }

    public void StopListening()
    {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        if (IsListening)
        {
            _stopSpeechRecognition();
            IsListening = false;
            Debug.Log("Speech recognition stopped");
        }
#endif
    }

    void OnDestroy()
    {
        StopListening();
    }
}


