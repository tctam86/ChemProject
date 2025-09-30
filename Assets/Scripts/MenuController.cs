using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    private SaveController saveController;

    void Start()
    {
        menuCanvas.SetActive(false);
        saveController = FindFirstObjectByType<SaveController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Press Tab to open menu
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            menuCanvas.SetActive(!menuCanvas.activeSelf);
        }
    }

    // These methods can be called by Unity UI buttons
    public void SaveGame()
    {
        if (saveController != null)
        {
            saveController.SaveGame();
        }
    }

    public void LoadGame()
    {
        if (saveController != null)
        {
            saveController.LoadGame();
        }
    }
}
