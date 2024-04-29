using System;
using UnityEngine;
using TMPro;

public class DateTimePicker : MonoBehaviour
{
    [SerializeField] public TMP_Dropdown monthDropdown;
    [SerializeField] public TMP_Dropdown dayDropdown;
    [SerializeField] public TMP_Dropdown hourDropdown;
    [SerializeField] public TMP_Dropdown minuteDropdown;

    public DateTime GetSelectedDateTime()
    {
        int year = DateTime.Now.Year; // Assuming current year or provide a way to select year
        int month = monthDropdown.value + 1;
        int day = dayDropdown.value + 1;
        int hour = hourDropdown.value;
        int minute = minuteDropdown.value;
        return new DateTime(year, month, day, hour, minute, 0);
    }
}