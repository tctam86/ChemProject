public enum DifficultyLevel
{
    Easy,
    Normal,
    Hard
}

[System.Serializable]
public class Question
{
    public string questionText;
    public string answer;
    public string difficulty;


    public Question(string question, string ans, DifficultyLevel diff)
    {
        questionText = question;
        answer = ans.ToUpper().Replace(" ", "");
        difficulty = diff.ToString();
    }


    public Question() { }

    public DifficultyLevel GetDifficultyLevel()
    {
        return difficulty switch
        {
            "Easy" => DifficultyLevel.Easy,
            "Normal" => DifficultyLevel.Normal,
            "Hard" => DifficultyLevel.Hard,
            _ => DifficultyLevel.Easy
        };
    }
}
