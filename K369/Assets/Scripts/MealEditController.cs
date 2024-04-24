using UnityEngine;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Globalization;
using Firebase.Database;
using System.Collections.Generic;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class MealEditPrefabController : MonoBehaviour
{
    public Text nameText;
    public Text descriptionText;
    public Button completeButton;
    public Button removeButton;
    public Image background;
    private string mealId;
    private int Points;

    private string MainScreenName = "Main screen";

    public void Initialize(string id, string name, string description, int points, bool isRemoveButtonActive)
    {
        mealId = id;
        nameText.text = name;
        descriptionText.text = description;
        Points = points;
        completeButton.GameObject().SetActive(false);
        removeButton.GameObject().SetActive(true);
    }

    public string SetMealId(string id)
    {
        mealId = id;
        return mealId;
    }


    public void OnDeleteButton()
    {       
        User user = UserManager.Instance.CurrentUser;
        string mealIdToRemove = FindMealToDelete(user, nameText.text, descriptionText.text);
        if (mealIdToRemove != null)
        {
            DeleteMealFromDatabase(user.Id, mealIdToRemove);
            Debug.Log("Deleted meal id: " + mealIdToRemove);

            DeletePrefab(user);
        }

        SceneManager.LoadScene(MainScreenName);
    }

    // Delete prefab associated with meal
    private void DeletePrefab(User user)
    {
        // After deleting the meal, update the user's meals
        DatabaseManager.Instance.GetUserMeals(user.Id, updatedMeals =>
        {
            if (updatedMeals != null)
            {
                user.Meals = updatedMeals;
            }
        });
    }

    // Finds meal to delete based on meal name and description
    private string FindMealToDelete(User user, string mealName, string mealDescription)
    {
        string mealId = "";
        if (user != null && user.Meals != null && user.Meals.Count > 0)
        {
            foreach (Meal meal in user.Meals)
            {
                if (meal.Name.Equals(mealName) &&  meal.Description.Equals(mealDescription))
                {
                    mealId = meal.MealId;
                    break;
                }
            }
        }
        return mealId;
    }

    private void DeleteMealFromDatabase(string userId, string mealId)
    {
        DatabaseManager.Instance.DeleteMeal(userId, mealId);
    }

}