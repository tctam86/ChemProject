using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    public void ActivateTab(int tabNo)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.grey;
        }
        pages[tabNo].SetActive(true);
        tabImages[tabNo].color = Color.white;
    }
}
