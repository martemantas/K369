using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TextButton : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onClick;
    public TextMeshProUGUI previousDayText;
    public TextMeshProUGUI nextDayText;

    private void Start()
    {
        previousDayText.text = string.Format("<- {0:MM/dd}", DateTime.Now.AddDays(-1));
        nextDayText.text = string.Format("{0:MM/dd} ->", DateTime.Now.AddDays(1));
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (name == "PreviousDay")
        {
            DateTime previousDate = GetCurrentDate(previousDayText.text);
            previousDate = previousDate.AddDays(-1);
            previousDayText.text = string.Format("<- {0:MM/dd}", previousDate);
            nextDayText.text = string.Format("{0:MM/dd} ->", previousDate.AddDays(2));
        }
        else if (name == "NextDay")
        {
            DateTime nextDate = GetCurrentDate(nextDayText.text);
            nextDate = nextDate.AddDays(1);
            previousDayText.text = string.Format("<- {0:MM/dd}", nextDate.AddDays(-2));
            nextDayText.text = string.Format("{0:MM/dd} ->", nextDate);
        }

        onClick.Invoke();
    }

    private DateTime GetCurrentDate(string text)
    {
        string dateString;
        if (text.StartsWith("<-"))
        {
            dateString = text.Substring(3, 5); // For previousDay text
        }
        else
        {
            dateString = text.Substring(0, 5); // For nextDay text
        }
        return DateTime.ParseExact(dateString, "MM/dd", null);
    }
}
