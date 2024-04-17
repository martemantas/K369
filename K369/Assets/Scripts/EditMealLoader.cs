using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EditMealLoader : MonoBehaviour
{
    public GameObject prefabToInstantiate;
    public Transform prefabParent;

    private string NutritionScreenName = "Nutrition screen";
    public ScrollRect scrollView;
    public Transform contentContainer;

    private void Start()
    {
        SpawnUserMeals();
    }

    public void SpawnUserMeals()
    {
        User user = UserManager.Instance.CurrentUser;

        if (user != null && user.Meals != null && user.Meals.Count > 0)
        {
            foreach (Meal meal in user.Meals)
            {
                GameObject mealInstance = Instantiate(prefabToInstantiate, prefabParent);
                MealPrefabController controller = mealInstance.GetComponent<MealPrefabController>();
                if (controller != null)
                {
                    controller.Initialize(meal.MealId, meal.Name, meal.Description, meal.Points);
                    if (meal.Completed)
                    {
                        controller.MarkCompleted();
                    }
                }
            }
        }
        else
        {
            Debug.Log("User has no meals or user is null.");
        }
    }

    public void OnExitButton()
    {
        SceneManager.LoadScene(NutritionScreenName);
    }

    public void OnEditButton()
    {
        ResetContent();
        SpawnUserMeals();
    }

    private void ResetContent()
    {
        // Loop through all child elements of the content container and destroy them
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        // Optionally, reset the scroll position to the top
        if (scrollView != null)
        {
            scrollView.normalizedPosition = Vector2.up;
        }
    }

}