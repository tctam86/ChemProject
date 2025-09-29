using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;

    void Start()
    {
        menuCanvas.SetActive(false);
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
}
