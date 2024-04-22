using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MealLoader : MonoBehaviour
{
    public GameObject prefabToInstantiate;
    public Transform prefabParent;

    private bool isCompleteButtonActive = true;

    public ScrollRect scrollView;
    public Transform contentContainer;

    private void Start()
    {
        SpawnUserMeals();
    }

    public void SpawnUserMeals()
    {
        ResetContent();
        
        User user = UserManager.Instance.CurrentUser;

        if (user != null && user.Meals != null && user.Meals.Count > 0)
        {
            foreach (Meal meal in user.Meals)
            {
                GameObject mealInstance = Instantiate(prefabToInstantiate, prefabParent);
                MealPrefabController controller = mealInstance.GetComponent<MealPrefabController>();
                if (controller != null)
                {
                    controller.Initialize(meal.MealId, meal.Name, meal.Description, meal.Points, isCompleteButtonActive);
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

    // Delete prefabs
    public void ResetContent()
    {
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