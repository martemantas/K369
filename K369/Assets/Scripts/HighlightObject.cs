using UnityEngine;
using TMPro;

public class ShowOnToday : MonoBehaviour
{
    public GameObject imageObject;
    public TMP_Text dayText;
    void Update()
    {
        string dayString = dayText.text.Trim();

        if (IsToday(dayString))
        {
            imageObject.SetActive(true);
        }
        else
        {
            imageObject.SetActive(false);
        }
    }

    bool IsToday(string day)
    {
        string today = System.DateTime.Today.Day.ToString();
        return day == today;
    }
}
