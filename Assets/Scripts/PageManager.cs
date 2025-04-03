using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rename the script to something more appropriate, e.g., PageManager
public class PageManager : MonoBehaviour
{
    // Assign all your page GameObjects here in the Inspector
    public List<GameObject> pages;

    // Index of the currently active page in the 'pages' list
    private int currentPageIndex = 0;

    // Optional delay before switching pages
    public float switchDelay = 0f; // Set to 0 for immediate switching

    void Start()
    {
        // Initialize: Make sure only the first page is active initially
        // and all others are inactive.
        if (pages == null || pages.Count == 0)
        {
            Debug.LogError("PageManager: No pages assigned in the Inspector!");
            return;
        }

        for (int i = 0; i < pages.Count; i++)
        {
            // Activate the page at the starting index, deactivate others
            pages[i].SetActive(i == currentPageIndex);
        }
    }

    // Call this function from your "Next" button's OnClick event
    public void ShowNextPage()
    {
        StartCoroutine(SwitchPageCoroutine(1)); // 1 means move forward
    }

    // Call this function from your "Previous" button's OnClick event
    public void ShowPreviousPage()
    {
        StartCoroutine(SwitchPageCoroutine(-1)); // -1 means move backward
    }

    // Coroutine to handle the page switch with optional delay
    private IEnumerator SwitchPageCoroutine(int direction)
    {
        if (pages.Count <= 1) yield break; // No switching needed if 0 or 1 page

        // Wait for the delay, if any
        if (switchDelay > 0)
        {
            yield return new WaitForSeconds(switchDelay);
        }

        // Deactivate the current page
        pages[currentPageIndex].SetActive(false);

        // Calculate the next page index
        currentPageIndex += direction;

        // Wrap around using the modulo operator for next page
        if (currentPageIndex >= pages.Count)
        {
            currentPageIndex = 0; // Wrap to start
        }
        // Wrap around for previous page
        else if (currentPageIndex < 0)
        {
            currentPageIndex = pages.Count - 1; // Wrap to end
        }

        // Activate the new page
        pages[currentPageIndex].SetActive(true);

        Debug.Log("Switched to page: " + pages[currentPageIndex].name + " (Index: " + currentPageIndex + ")");
    }

    // --- Optional: Function to show a specific page by its index ---
    // You could call this directly if you have buttons for Page 1, Page 2, etc.
    public void ShowPageAtIndex(int indexToShow)
    {
        if (indexToShow >= 0 && indexToShow < pages.Count)
        {
            StartCoroutine(SwitchToSpecificPageCoroutine(indexToShow));
        }
        else
        {
            Debug.LogWarning("ShowPageAtIndex: Invalid index " + indexToShow);
        }
    }

    private IEnumerator SwitchToSpecificPageCoroutine(int indexToShow)
    {
        if (currentPageIndex == indexToShow) yield break; // Already on the target page

        if (switchDelay > 0)
        {
            yield return new WaitForSeconds(switchDelay);
        }

        // Deactivate current
        pages[currentPageIndex].SetActive(false);

        // Set and activate target
        currentPageIndex = indexToShow;
        pages[currentPageIndex].SetActive(true);

        Debug.Log("Switched directly to page: " + pages[currentPageIndex].name + " (Index: " + currentPageIndex + ")");
    }
}
