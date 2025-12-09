using UnityEngine;

public static class GameDifficulty
{
    public static string CurrentDifficulty
    {
        get { return PlayerPrefs.GetString("GameDifficulty", "Easy"); }
    }

    public static int GetDifficultyMultiplier()
    {
        switch (CurrentDifficulty)
        {
            case "Easy":
                return 1;
            case "Medium":
                return 2;
            case "Hard":
                return 3;
            default:
                return 1;
        }
    }
}