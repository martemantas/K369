using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadioButtonGroup : MonoBehaviour
{
    public Button[] buttons;
    private Button selectedButton;
    public Sprite selectedSprite; 
    public Sprite deselectedSprite; 

    void Start()
    {
        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => SelectButton(btn));
        }
    }

    void SelectButton(Button button)
    {
        if (selectedButton != null)
        {
            selectedButton.GetComponent<Image>().sprite = deselectedSprite;
        }

        selectedButton = button;
        selectedButton.GetComponent<Image>().sprite = selectedSprite;
    }
}
