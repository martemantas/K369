using UnityEngine;
using TMPro;
using System;

public class DayOfWeekAbbreviation : MonoBehaviour
{
    public TMP_Text textDisplay;
    public bool dayFlag = true; // Default to adding days
    public bool stringFlag = true; // Default to returning abbreviated day as string
    public int daysOff = 0;

    void Start()
    {
        string result = GetDay();
        textDisplay.text = result;
    }

    string GetDay()
    {
        if (stringFlag)
        {
            // Get today's day of the week
            DayOfWeek today = System.DateTime.Today.DayOfWeek;

            // Adjust the day based on the dayFlag and daysOff parameters
            if (dayFlag)
            {
                today = (DayOfWeek)(((int)today + daysOff) % 7);
            }
            else
            {
                today = (DayOfWeek)(((int)today - daysOff) % 7);
                if (today < 0)
                {
                    today += 7; // Ensures the result is a positive value
                }
            }

            // Convert the adjusted day to its abbreviated form
            switch (today)
            {
                case DayOfWeek.Sunday:
                    return "Su";
                case DayOfWeek.Monday:
                    return "Mo";
                case DayOfWeek.Tuesday:
                    return "Tu";
                case DayOfWeek.Wednesday:
                    return "We";
                case DayOfWeek.Thursday:
                    return "Th";
                case DayOfWeek.Friday:
                    return "Fr";
                case DayOfWeek.Saturday:
                    return "Sa";
                default:
                    return "";
            }
        }
        else
        {
            // Adjust the day of the month based on daysOff
            int dayOfMonth = System.DateTime.Today.Day;

            if (dayFlag)
            {
                dayOfMonth += daysOff;
            }
            else
            {
                dayOfMonth -= daysOff;
                if (dayOfMonth < 1)
                {
                    // Calculate the correct day for previous month
                    System.DateTime previousMonth = System.DateTime.Today.AddMonths(-1);
                    int daysInPreviousMonth = System.DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
                    dayOfMonth += daysInPreviousMonth;
                }
            }

            return dayOfMonth.ToString();
        }
    }
}
