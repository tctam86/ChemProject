using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    [SerializeField] private DifficultyLevel levelDifficulty = DifficultyLevel.Easy;
    [SerializeField] private float startDelay = 1f;


    void Start()
    {
        Invoke(nameof(StartPuzzle), startDelay);
    }

    void StartPuzzle()
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
