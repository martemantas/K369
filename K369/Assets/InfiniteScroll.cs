using UnityEngine;
using UnityEngine.UI;
using TMPro; // Make sure to include the TMPro namespace

public class InfiniteScroll : MonoBehaviour
{
    public RectTransform contentPanel;
    public GameObject hourPrefab;
    public float itemHeight = 60; // Adjust according to your prefab size
    public int totalItems = 24; // Total hours
    private ScrollRect scrollRect;
    private int currentIndex = 0;

    void Update()
    {
        if (contentPanel.anchoredPosition.y < 0)
        {
            contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x, contentPanel.anchoredPosition.y + totalItems * itemHeight);
        }
        else if (contentPanel.anchoredPosition.y > totalItems * itemHeight - itemHeight)
        {
            contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x, contentPanel.anchoredPosition.y - totalItems * itemHeight);
        }
    }

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        InitializeHours();
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }

    void InitializeHours()
    {
        if (itemHeight <= 0 && hourPrefab.GetComponent<RectTransform>() != null)
        {
            itemHeight = hourPrefab.GetComponent<RectTransform>().rect.height;
        }

        contentPanel.sizeDelta = new Vector2(contentPanel.sizeDelta.x, totalItems * itemHeight);

        for (int i = 0; i < totalItems; i++)
        {
            GameObject hourItem = Instantiate(hourPrefab, contentPanel);
            hourItem.GetComponentInChildren<TMP_Text>().text = $"{i} Hour";
            hourItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -i * itemHeight);
        }
    }



    void OnScrollValueChanged(Vector2 position)
    {
        float newY = Mathf.Repeat(contentPanel.anchoredPosition.y, itemHeight * totalItems);
        contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x, newY);
        UpdateCurrentIndex();
    }

    void UpdateCurrentIndex()
    {
        currentIndex = Mathf.RoundToInt((contentPanel.anchoredPosition.y / itemHeight) % totalItems);
        Debug.Log("Current Hour: " + currentIndex);
    }
}