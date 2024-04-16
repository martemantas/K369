using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadioButtonGroup : MonoBehaviour
{
    public Button[] buttons;
    private Button selectedButton;

    private Color selectedColor = new Color(0.607f, 0.745f, 0.243f); // #9BBE3E
    private ColorBlock defaultColors; 

    void Start()
    {

        if (buttons.Length > 0)
        {
            defaultColors = buttons[0].colors;
        }

        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => SelectButton(btn));
        }
    }

    void SelectButton(Button button)
    {
        if (selectedButton != null)
        {
            selectedButton.colors = defaultColors;
        }

        selectedButton = button;

        ColorBlock cb = selectedButton.colors;
        cb.normalColor = selectedColor;
        cb.highlightedColor = selectedColor;
        cb.pressedColor = selectedColor;
        cb.selectedColor = selectedColor;

        selectedButton.colors = cb;
    }
    public string GetSelectedButtonText()
    {
        if (selectedButton != null)
        {
            return selectedButton.GetComponentInChildren<TextMeshProUGUI>().text;
        }
        else
        {
            return "";
        }
    }
}