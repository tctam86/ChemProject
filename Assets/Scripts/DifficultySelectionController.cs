using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DifficultySelectionController : MonoBehaviour
{
    public TMP_Dropdown difficultyDropdown;
    public Button startButton;

    private string selectedDifficulty = "Easy";

    void Start()
    {
        difficultyDropdown.ClearOptions();
        difficultyDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Easy",
            "Normal",
            "Hard"
        });

        // Set default value
        difficultyDropdown.value = 0;

        difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);

        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGameWithDifficulty);
        }

        Debug.Log($"Initial difficulty: {selectedDifficulty}");
    }

    void OnDifficultyChanged(int index)
    {
        Debug.Log($"Dropdown changed to index: {index}");

        switch (index)
        {
            case 0:
                selectedDifficulty = "Easy";
                break;
            case 1:
                selectedDifficulty = "Normal";
                break;
            case 2:
                selectedDifficulty = "Hard";
                break;
        }

        Debug.Log($"Selected difficulty: {selectedDifficulty}");
    }

    public void StartGameWithDifficulty()
    {
        Debug.Log($"Starting game with difficulty: {selectedDifficulty}");

        PlayerPrefs.SetString("GameDifficulty", selectedDifficulty);
        PlayerPrefs.Save();

        Debug.Log($"Saved to PlayerPrefs: {PlayerPrefs.GetString("GameDifficulty")}");

        SceneManager.LoadScene("MainGame");
    }

    void OnDestroy()
    {
        if (difficultyDropdown != null)
        {
            difficultyDropdown.onValueChanged.RemoveListener(OnDifficultyChanged);
        }
    }
}