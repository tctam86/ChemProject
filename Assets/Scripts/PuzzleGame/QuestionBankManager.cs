using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestionDatabase
{
    public List<Question> easyQuestions;
    public List<Question> normalQuestions;
    public List<Question> hardQuestions;
}

public class QuestionBankManager : MonoBehaviour
{
    public static QuestionBankManager Instance { get; private set; }

    [Header("JSON Settings")]
    [SerializeField] private string jsonFileName = "questions"; // File in Resources folder
    [SerializeField] private bool loadFromJSON = true;

    [Header("Question Bank")]
    private List<Question> easyQuestions = new List<Question>();
    private List<Question> normalQuestions = new List<Question>();
    private List<Question> hardQuestions = new List<Question>();

    [Header("References")]
    [SerializeField] private TileSpawner tileSpawner;
    [SerializeField] private PuzzleManager puzzleManager;
    [SerializeField] private QuestionOverlayController questionOverlayController;

    private Question currentQuestion;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;

            if (loadFromJSON)
            {
                LoadQuestionsFromJSON();
            }
            else
            {
                InitializeQuestionBankHardcoded();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadQuestionsFromJSON()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);

        if (jsonFile != null)
        {
            try
            {
                QuestionDatabase database = JsonUtility.FromJson<QuestionDatabase>(jsonFile.text);

                easyQuestions = database.easyQuestions ?? new List<Question>();
                normalQuestions = database.normalQuestions ?? new List<Question>();
                hardQuestions = database.hardQuestions ?? new List<Question>();

                Debug.Log($"Loaded questions from JSON: Easy={easyQuestions.Count}, Normal={normalQuestions.Count}, Hard={hardQuestions.Count}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error parsing JSON: {e.Message}");
                InitializeQuestionBankHardcoded();
            }
        }
        else
        {
            Debug.LogWarning($"JSON file '{jsonFileName}' not found in Resources folder. Using hardcoded questions.");
            InitializeQuestionBankHardcoded();
        }
    }

    private void InitializeQuestionBankHardcoded()
    {
        easyQuestions.Add(new Question("H₂O is the chemical formula for?", "WATER", DifficultyLevel.Easy));
        easyQuestions.Add(new Question("NaCl is commonly known as?", "SALT", DifficultyLevel.Easy));

        normalQuestions.Add(new Question("A substance that speeds up a reaction?", "CATALYST", DifficultyLevel.Normal));

        hardQuestions.Add(new Question("Study of carbon compounds?", "ORGANIC", DifficultyLevel.Hard));
    }

    public Question GetRandomQuestion(DifficultyLevel difficulty)
    {
        List<Question> questionPool = difficulty switch
        {
            DifficultyLevel.Easy => easyQuestions,
            DifficultyLevel.Normal => normalQuestions,
            DifficultyLevel.Hard => hardQuestions,
            _ => easyQuestions
        };

        if (questionPool.Count == 0)
        {
            Debug.LogError($"No questions for difficulty: {difficulty}");
            return null;
        }

        return questionPool[Random.Range(0, questionPool.Count)];
    }

    public void StartPuzzle(DifficultyLevel difficulty)
    {
        currentQuestion = GetRandomQuestion(difficulty);

        if (currentQuestion != null)
        {
            SetupPuzzle();
        }
    }

    private void SetupPuzzle()
    {

        questionOverlayController?.ShowQuestion(currentQuestion);
        puzzleManager?.SetCorrectAnswer(currentQuestion.answer);
        tileSpawner?.SpawnLetterTiles(currentQuestion.answer);
        if (puzzleManager != null && currentQuestion != null)
        {
            float timeLimit = currentQuestion.GetTimeLimit();
            puzzleManager.StartTimer(timeLimit);
        }
    }

    public Question GetCurrentQuestion() => currentQuestion;
}
