using System.Collections.Generic;
using UnityEngine;

public class PuzzleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject letterTilePrefab;
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private PuzzleManager puzzleManager;

    private List<char> answerLetters = new List<char>();

    public void SpawnPuzzle(string answer)
    {
        // Clear any existing tiles
        ClearExistingTiles();

        // Set the correct answer in the puzzle manager
        puzzleManager.SetCorrectAnswer(answer);

        // Convert answer to character list
        answerLetters.Clear();
        foreach (char c in answer.ToUpper())
        {
            answerLetters.Add(c);
        }

        List<char> lettersToSpawn = new List<char>(answerLetters);


        while (lettersToSpawn.Count < spawnPositions.Length)
        {
            char randomLetter = (char)('A' + Random.Range(0, 26));
            lettersToSpawn.Add(randomLetter);
        }

        ShuffleLetters(lettersToSpawn);


        for (int i = 0; i < spawnPositions.Length && i < lettersToSpawn.Count; i++)
        {
            GameObject tile = Instantiate(letterTilePrefab, spawnPositions[i].position, Quaternion.identity);
            LetterTile letterTile = tile.GetComponent<LetterTile>();
            if (letterTile != null)
            {
                letterTile.SetLetter(lettersToSpawn[i].ToString());
            }
        }
    }

    private void ClearExistingTiles()
    {
        LetterTile[] existingTiles = FindObjectsByType<LetterTile>(FindObjectsSortMode.None);
        foreach (LetterTile tile in existingTiles)
        {
            Destroy(tile.gameObject);
        }
    }

    private void ShuffleLetters(List<char> letters)
    {
        for (int i = 0; i < letters.Count; i++)
        {
            char temp = letters[i];
            int randomIndex = Random.Range(i, letters.Count);
            letters[i] = letters[randomIndex];
            letters[randomIndex] = temp;
        }
    }
}