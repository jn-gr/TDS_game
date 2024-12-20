using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaNavigation : MonoBehaviour
{
    public GameObject[] pages; // Array to store the panels
    public Button leftButton;  // Reference to the left button
    public Button rightButton; // Reference to the right button

    int currentPageIndex = 0; // Track the currently active page

    void Start()
    {
        UpdateUI();
    }

    public void ShowPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            pages[currentPageIndex].SetActive(false); // Hide current page
            currentPageIndex--; // Move to the previous page
            pages[currentPageIndex].SetActive(true); // Show the new page
            UpdateUI();
        }
    }

    public void ShowNextPage()
    {
        if (currentPageIndex < pages.Length - 1)
        {
            pages[currentPageIndex].SetActive(false); // Hide current page
            currentPageIndex++; // Move to the next page
            pages[currentPageIndex].SetActive(true); // Show the new page
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        // Enable or disable buttons based on the current page
        leftButton.interactable = currentPageIndex > 0;
        rightButton.interactable = currentPageIndex < pages.Length - 1;
    }
}
