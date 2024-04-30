using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DropDownPopulator : MonoBehaviour
{
    public TMP_Dropdown monthDropdown;
    public TMP_Dropdown dayDropdown;
    public TMP_Dropdown hourDropdown;
    public TMP_Dropdown minuteDropdown;

    private void Start()
    {
        PopulateMonthDropdown();
        int currentMonth = System.DateTime.Now.Month;
        monthDropdown.value = currentMonth - 1;  

        int daysInCurrentMonth = System.DateTime.DaysInMonth(System.DateTime.Now.Year, currentMonth);
        PopulateDayDropdown(daysInCurrentMonth);
        dayDropdown.value = System.DateTime.Now.Day - 1;  

        PopulateHourDropdown();
        hourDropdown.value = 23;  

        PopulateMinuteDropdown();
        minuteDropdown.value = 59;  

        monthDropdown.onValueChanged.AddListener(delegate { UpdateDayDropdown(); });
    }

    private void PopulateMonthDropdown()
    {
        List<string> months = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                                                   "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        monthDropdown.ClearOptions();
        monthDropdown.AddOptions(months);
    }

    private void PopulateDayDropdown(int days)
    {
        List<string> dayNumbers = new List<string>();
        for (int i = 1; i <= days; i++)
        {
            dayNumbers.Add(i.ToString());
        }
        dayDropdown.ClearOptions();
        dayDropdown.AddOptions(dayNumbers);
    }

    private void PopulateHourDropdown()
    {
        List<string> hours = new List<string>();
        for (int i = 0; i < 24; i++)
        {
            hours.Add(i.ToString("D2"));  
        }
        hourDropdown.ClearOptions();
        hourDropdown.AddOptions(hours);
    }

    private void PopulateMinuteDropdown()
    {
        List<string> minutes = new List<string>();
        for (int i = 0; i < 60; i++)
        {
            minutes.Add(i.ToString("D2"));  
        }
        minuteDropdown.ClearOptions();
        minuteDropdown.AddOptions(minutes);
    }

    private void UpdateDayDropdown()
    {
        int month = monthDropdown.value + 1;
        int year = System.DateTime.Now.Year;
        int daysInMonth = System.DateTime.DaysInMonth(year, month);
        PopulateDayDropdown(daysInMonth);
        dayDropdown.value = System.DateTime.Now.Day - 1;
    }
}
