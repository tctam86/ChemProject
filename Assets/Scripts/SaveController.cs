using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveController : MonoBehaviour
{
    private string saveLocation;

    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "DataSave.json");

        // Check if we need to restore position after scene loads
        StartCoroutine(CheckForPositionRestore());
    }

    IEnumerator CheckForPositionRestore()
    {
        // Wait one frame to ensure scene is fully loaded
        yield return null;

        if (PlayerPrefs.GetInt("ShouldRestorePosition", 0) == 1)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 savedPos = new Vector3(
                    PlayerPrefs.GetFloat("RestoreX"),
                    PlayerPrefs.GetFloat("RestoreY"),
                    PlayerPrefs.GetFloat("RestoreZ")
                );
                player.transform.position = savedPos;
                Debug.Log($"Player position restored to: {savedPos}");
            }
            // Clear the flag
            PlayerPrefs.SetInt("ShouldRestorePosition", 0);
        }
    }

    public void SaveGame()
    {
        // Find the player in the current scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("No player found with tag 'Player'!");
            return;
        }

        // Create save data
        DataSave dataSave = new DataSave
        {
            playerPosition = player.transform.position,
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex,
            currentSceneName = SceneManager.GetActiveScene().name,
        };

        // Convert to JSON and save to file
        string jsonData = JsonUtility.ToJson(dataSave, true);
        File.WriteAllText(saveLocation, jsonData);

        Debug.Log($"Game saved! Scene: {dataSave.currentSceneName} Position: {dataSave.playerPosition}");
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            DataSave saveData = JsonUtility.FromJson<DataSave>(File.ReadAllText(saveLocation));

            Debug.Log($"Loading to scene: {saveData.currentSceneName} at position: {saveData.playerPosition}");

            PlayerPrefs.SetFloat("RestoreX", saveData.playerPosition.x);
            PlayerPrefs.SetFloat("RestoreY", saveData.playerPosition.y);
            PlayerPrefs.SetFloat("RestoreZ", saveData.playerPosition.z);
            PlayerPrefs.SetInt("ShouldRestorePosition", 1);


            SceneManager.LoadScene(saveData.currentSceneIndex);
        }
        else
        {
            SaveGame();
        }
    }

}
