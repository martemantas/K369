using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR;
using Unity.VisualScripting;
using System.Globalization;
using System.Collections.Generic;

public class FormToPrefabSubmitterMeals : MonoBehaviour
{
    public TMP_InputField[] inputFields;
    public GameObject prefabToInstantiate;
    public Transform prefabParent;

    private int carbohydrates = 0;
    private int proteins = 0;
    private int fats = 0;
    private int kcal = 0;
    private int pointsToAdd = 15;

    public EditMealLoader loader;
    public ScrollRect scrollView;
    public Transform contentContainer;

    public Toggle allDay;
    public TMP_InputField endHoursInput;
    public TMP_InputField endMinutesInput;

    public TMP_Dropdown dropdownTodayTommorowSelection;
    private int selectionValue;

    private string endHour;
    private string endMinutes;
    private string endDate;
    private string dateAdded;
    private string dateExpire;
    private string mealName;
    private string mealDescription;

    public GameObject addNewMealScreen;
    public GameObject errorMessage;

    private void Start()
    {
        string currentHour = DateTime.Now.AddHours(1).ToString("HH");
        endHoursInput.text = currentHour;
        string currentMin = DateTime.Now.ToString("mm");
        endMinutesInput.text = currentMin;
        dropdownTodayTommorowSelection.onValueChanged.AddListener(HandleSelectionValue);
    }

    public void OnSubmitButtonClick()
    {
        SetValues(); // set primary values
        Validate();
    }

    private void SetValues()
    {
        mealName = inputFields[0].text;
        endHour = endHoursInput.text;
        endMinutes = endMinutesInput.text;

        if (selectionValue == 0) // meal for today
        {
            if (allDay.isOn) // meal for all day
            {
                dateAdded = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                dateExpire = SetNextDayMidnight(1);
            }
            else // meal for set time
            {
                dateAdded = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                endDate = DateTime.Now.ToString("yyyy-MM-dd");
                dateExpire = endDate + " " + endHour.PadLeft(2, '0') + ":" + endMinutes.PadLeft(2, '0');

            }
        }
        if (selectionValue == 1) // meal for tomorrow
        {
            if (allDay.isOn) // meal for all day
            {
                dateAdded = SetNextDayMidnight(1);
                dateExpire = SetNextDayMidnight(2);
            }
            else // meal for set time
            {
                dateAdded = SetNextDayMidnight(1);
                endDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                dateExpire = endDate + " " + endHour.PadLeft(2, '0') + ":" + endMinutes.PadLeft(2, '0');
            }
        }

        mealDescription = "Complete until ";
        if (allDay.isOn)
        {
            mealDescription += "midnight";
        }
        else
        {
            mealDescription += endHour.PadLeft(2, '0') + ":" + endMinutes.PadLeft(2, '0');
        }
        mealDescription += "\n" + inputFields[1].text;
        if (inputFields[1].text.Length != 0)
        {
            mealDescription += "\n";
        }

    }

    private string SetNextDayMidnight(int daysToAdd)
    {
        DateTime currentDate = DateTime.Now;
        DateTime zeroTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0);
        DateTime nextDay = zeroTime.AddDays(daysToAdd);
        string formattedDate = nextDay.ToString("yyyy-MM-dd HH:mm");
        return formattedDate;
    }

    private void Validate()
    {
        if (mealName.Length > 0 && !MealExists(mealName, mealDescription))
        {
            InstantiateNewMeal(); // create new meal
            addNewMealScreen.SetActive(false);
        }
        else
        {
            endHoursInput.text = DateTime.Now.AddHours(1).ToString("HH");
            endMinutesInput.text = DateTime.Now.ToString("mm");
            errorMessage.SetActive(true);
        }
    }

    private bool MealExists(string mealName, string mealDescription)
    {
        User user = UserManager.Instance.CurrentUser;
        foreach (Meal meal in user.Meals)
        {
            if (meal.Name.Equals(mealName) && meal.Description.Equals(mealDescription))
            {
                return true;
            }
        }
        return false;
    }

    // Create meal, add to the database and show to user
    private void InstantiateNewMeal()
    {
        GameObject instantiatedPrefab = Instantiate(prefabToInstantiate, prefabParent ? prefabParent : null);
        User user = UserManager.Instance.CurrentUser;
        string mealId = Guid.NewGuid().ToString();

        if (inputFields[0].text.Length == 0)
        {
            inputFields[0].text = "Meal name";
        }
        if (inputFields[1].text.Length == 0)
        {
            inputFields[1].text = string.Empty;
        }

        user.Meals.Add(new Meal(mealId, mealName, mealDescription, carbohydrates,
                                proteins, fats, false, dateAdded, "", dateExpire, pointsToAdd, kcal));
        if (user.userType == 1)
        {
            DatabaseManager.Instance.AddNewMeal(user.Id, mealId, mealName, mealDescription, carbohydrates,
                                proteins, fats, false, dateAdded, "", dateExpire, pointsToAdd, kcal);
        }
        else if (user.userType == 2)
        {
            string childID = UserManager.Instance.GetSelectedChildToViewID();
            DatabaseManager.Instance.AddNewMealForChild(childID, mealId, mealName, mealDescription, carbohydrates,
                                proteins, fats, false, dateAdded, "", dateExpire, pointsToAdd, kcal);
        }

        MealPrefabController controller = instantiatedPrefab.GetComponent<MealPrefabController>();
        if (controller != null)
        {
            controller.Initialize(mealId, mealName, mealDescription, pointsToAdd, true);
        }

        MealEditPrefabController controllerEdit = instantiatedPrefab.GetComponent<MealEditPrefabController>();
        if (controllerEdit != null)
        {
            controllerEdit.Initialize(mealId, mealName, mealDescription, pointsToAdd, true);
            controllerEdit.SetMealId(mealId);
        }

        // if user is not guest
        if (user.userType != 0)
        {
            // Load prefabs
            ResetContent(contentContainer, scrollView);
            loader.Spawn();
        }
    }

    // Delete prefabs
    private void ResetContent(Transform container, ScrollRect scroll)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // Optionally, reset the scroll position to the top
        if (scroll != null)
        {
            scrollView.normalizedPosition = Vector2.up;
        }
    }

    public void HandleSelectionValue(int val)
    {
        if (val == 0)
        {
            selectionValue = 0;
        }
        if (val == 1)
        {
            selectionValue = 1;
        }
    }

}

