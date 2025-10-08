using UnityEngine;

public class PuzzleBootstrapper : MonoBehaviour
{
    public PuzzleSpawner puzzleSpawner;

    public string answer;

    void Start()
    {
        if (puzzleSpawner != null)
        {
            puzzleSpawner.SpawnPuzzle(answer);
        }
        else
        {
            Debug.LogError("PuzzleSpawner is not assigned in the PuzzleBootstrapper.");
        }
    }
}
