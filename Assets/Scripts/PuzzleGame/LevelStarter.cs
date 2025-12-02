using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    [SerializeField] private DifficultyLevel levelDifficulty = DifficultyLevel.Easy;
    [SerializeField] private float startDelay = 1f;


    // Removed automatic start from Start() method
    // The game will now be started manually by calling StartPuzzle()

    public void StartPuzzle()
    {
        if (QuestionBankManager.Instance != null)
        {
            QuestionBankManager.Instance.StartPuzzle(levelDifficulty);
        }
        else
        {
            Debug.LogError("Not found");
        }
    }


}
