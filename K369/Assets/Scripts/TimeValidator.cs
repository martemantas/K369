using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeValidator : MonoBehaviour
{
    public TMP_InputField inputFieldHour;
    public TMP_InputField inputFieldMinute;
    private string timeText;
    private string timeText2;

    public void ValidateHour()
    {
        timeText = inputFieldHour.text;

        if (timeText.Length > 2 || timeText.Length == 0 ||
            int.Parse(timeText) < 0 || int.Parse(timeText) > 23)
        {
            inputFieldHour.text = DateTime.Now.AddHours(1).ToString("HH");         
        }
        else
        {
            inputFieldHour.text = timeText;
        }
    }

    public void ValidateMinute()
    {
        timeText2 = inputFieldMinute.text;
        if (timeText2.Length > 2 || timeText2.Length == 0 ||
            int.Parse(timeText2) < 0 || int.Parse(timeText2) > 59)
        {
            inputFieldMinute.text = DateTime.Now.ToString("mm");
        }
        else
        {
            inputFieldMinute.text = timeText2;
        }
    }

    // Default completion time is one hour after
    private string FormatCurrentTime(int time)
    {
        string output = "";
        if (time + 1 < 10)
        {
            output = "0" + (time + 1).ToString();
        }
        else
        {
            output = (time + 1).ToString();
        }
        return output;
    }

}
