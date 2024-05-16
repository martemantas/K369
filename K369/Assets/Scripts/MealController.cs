using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MealPrefabController : MonoBehaviour
{
    public Text nameText;
    public Text descriptionText;
    public Button completeButton;
    public Button removeButton;
    public Image background;
    public string mealId;
    private int Points;

    [FormerlySerializedAs("_cookiePopupText")][SerializeField] private GameObject _PopupText;


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
        User user;
        User currentUser = UserManager.Instance.CurrentUser;

        if (currentUser.userType == 2) // parent
        {
            user = currentUser.child;
        }
        else // child or guest
        {
            user = currentUser;

            Action<Meal> mealCallback = (meal) =>
            {
                if (meal != null)
                {
                    meal.Completed = true;
                    DatabaseManager.Instance.MarkMealAsCompleted(user.Id, mealId);
                    if (!meal.pointsGiven)
                    {
                        meal.pointsGiven = true;
                        user.Points += Points;
                        DatabaseManager.Instance.UpdateUserPoints(user.Id, user.Points);

                        Debug.Log("meal calories: " + meal.Calories);
                        user.todayCalories += meal.Calories;
                        user.todayProtein += meal.Protein;
                        user.todayFat += meal.Fat;
                        user.todayCarbs += meal.Carbohydrates;
                        DatabaseManager.Instance.UpdateUserNutritionalValues(user.Id, user.todayCalories, user.todayProtein,
                                                                             user.todayFat, user.todayCarbs);
                    }
                }
                MarkCompleted();

                GameObject textPrefab = Instantiate(_PopupText, gameObject.transform);
                textPrefab.GetComponent<PopupText>().Setup(Points);
            };
            DatabaseManager.Instance.GetMealById(user.Id, mealId, mealCallback);
        }

        
    }




}



