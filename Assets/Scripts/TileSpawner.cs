using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [SerializeField] private GameObject letterTilePrefab;
    [SerializeField] private Transform mapTileParent;
    [SerializeField] private float tileSpacing = 1.0f;

    private List<Transform> mapTiles = new List<Transform>();

    void Start()
    {
        // Collect all map tiles
        foreach (Transform child in mapTileParent)
        {
            mapTiles.Add(child);
        }
    }

    public void SpawnLetterTiles(string answer)
    {
        // Shuffle map tiles to randomize spawn positions
        Shuffle(mapTiles);

        // Spawn letter tiles in a line on random map tiles
        for (int i = 0; i < answer.Length; i++)
        {
            if (i < mapTiles.Count)
            {
                Vector3 spawnPosition = mapTiles[i].position + Vector3.up * tileSpacing;
                GameObject tile = Instantiate(letterTilePrefab, spawnPosition, Quaternion.identity);
                LetterTile letterTile = tile.GetComponent<LetterTile>();
                if (letterTile != null)
                {
                    letterTile.SetLetter(answer[i].ToString());
                }
            }
        }
    }

    private void Shuffle(List<Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Transform temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}