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

    private string startTime;
    private string endHour;
    private string endMinutes;
    private string endDate;
    private string dateAdded;
    private string dateExpire;
    private string mealName;
    private string mealDescription;

    private void Start()
    {
        endHoursInput.text = DateTime.Now.AddHours(1).ToString("HH");
        endMinutesInput.text = DateTime.Now.ToString("mm");
        selectionValue = 0;
    }

    public void OnSubmitButtonClick()
    {
        SetValues(); // set primary values
        if (mealName.Length > 0 && !MealExists(mealName, mealDescription))
        {
            InstantiateNewMeal(); // create new meal


        }
        else
        {
            // show popup message
        }
 
    }

    private void SetValues()
    {
        mealName = inputFields[0].text;
        mealDescription = inputFields[1].text;
    
        int addDay = 0;
        if (allDay.isActiveAndEnabled)  // Set end to the midnight
        {
            endHoursInput.text = DateTime.Now.ToString("00");
            endMinutesInput.text = DateTime.Now.ToString("00");
            addDay = 1;
        }

        if (selectionValue == 0)
        {
            startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            endHour = endHoursInput.text;
            endMinutes = endMinutesInput.text;
            endDate = DateTime.Now.AddDays(addDay).ToString("yyyy-MM-dd");
            dateAdded = startTime; 
            dateExpire = endDate + " " + endHour + ":" + endMinutes; 
        }
        else
        {
            startTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm");
            endHour = endHoursInput.text;
            endMinutes = endMinutesInput.text;
            endDate = DateTime.Now.AddDays(addDay+1).ToString("yyyy-MM-dd");
            dateAdded = startTime; 
            dateExpire = endDate + " " + endHour + ":" + endMinutes; 
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

        user.Meals.Add(new Meal(mealId, inputFields[0].text, inputFields[1].text, carbohydrates,
                                proteins, fats, false, dateAdded, "", dateExpire, pointsToAdd, kcal));
        if (user.userType != 0)
        {
            DatabaseManager.Instance.AddNewMeal(user.Id, mealId, inputFields[0].text, inputFields[1].text, carbohydrates,
                                proteins, fats, false, dateAdded, "", dateExpire, pointsToAdd, kcal);
        }

        MealPrefabController controller = instantiatedPrefab.GetComponent<MealPrefabController>();
        if (controller != null)
        {
            controller.Initialize(mealId, inputFields[0].text, inputFields[1].text, pointsToAdd, true);
        }

        MealEditPrefabController controllerEdit = instantiatedPrefab.GetComponent<MealEditPrefabController>();
        if (controllerEdit != null)
        {
            controllerEdit.Initialize(mealId, inputFields[0].text, inputFields[1].text, pointsToAdd, true);
            controllerEdit.SetMealId(mealId);
        }

        // if user is not guest
        if (user.userType != 0)
        {
            // Load prefabs
            ResetContent(contentContainer, scrollView);
            loader.SpawnUserMeals();
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
            Debug.Log("selected 0");
        }
        if (val == 1)
        {
            selectionValue = 1;
            Debug.Log("selected 1");
        }
    }

}

