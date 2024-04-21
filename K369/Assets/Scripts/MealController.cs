using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MealPrefabController : MonoBehaviour
{
    public Text nameText;
    public Text descriptionText;
    public Button completeButton;
    public Button removeButton;
    public Image background;
    private string mealId;
    private int Points;


    public void Initialize(string id, string name, string description, int points, bool isCompleteButtonActive)
    {
        mealId = id;
        nameText.text = name;
        descriptionText.text = description;
        Points = points;
        completeButton.GameObject().SetActive(true);
        removeButton.GameObject().SetActive(false);
    }

    public void MarkCompleted()
    {
        completeButton.GameObject().SetActive(false);
        background.color = new Color(0.6333f, 0.8392f, 0.5294f, 0.7098f);     
    }

    public void OnCompleteButton()
    {
        Debug.Log("Complete pressed");
        User user = UserManager.Instance.CurrentUser;
        Meal meal = user.Meals.Find(t => t.MealId == mealId);
        if (meal != null)
        {
            meal.Completed = true;
            DatabaseManager.Instance.MarkMealAsCompleted(user.Id, mealId);
            if (!meal.pointsGiven)
            {
                meal.pointsGiven = true;
                UserManager.Instance.CurrentUser.Points += Points;
                DatabaseManager.Instance.UpdateUserPoints(UserManager.Instance.CurrentUser.Id, UserManager.Instance.CurrentUser.Points);
            }
        }
        MarkCompleted();       
    }




}



