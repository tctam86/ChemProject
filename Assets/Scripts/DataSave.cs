using UnityEngine;

[System.Serializable]
//Pack or Unpack data
public class DataSave
{
    public Vector3 playerPosition;
    public int currentSceneIndex;           // Which scene the player was in
    public string currentSceneName;         // Scene name as backup

}
