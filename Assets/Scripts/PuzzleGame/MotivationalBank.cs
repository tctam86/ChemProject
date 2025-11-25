using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MotivationalMessageDatabase
{
    public List<string> messages;
}

public class MotivationalBank : MonoBehaviour
{
    public static MotivationalBank Instance { get; private set; }
    private List<string> motivationalMessages = new List<string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadMessagesFromJSON();
    }

    private void LoadMessagesFromJSON()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("motivationalMessages");
        if (jsonFile != null)
        {
            MotivationalMessageDatabase db = JsonUtility.FromJson<MotivationalMessageDatabase>(jsonFile.text);
            motivationalMessages = db.messages ?? new List<string>();
        }
        else
        {
            motivationalMessages.Add("Stay determined.");
        }
    }

    public string GetRandomMessage()
    {
        if (motivationalMessages == null || motivationalMessages.Count == 0)
            return "Stay determined.";
        return motivationalMessages[Random.Range(0, motivationalMessages.Count)];
    }
}
