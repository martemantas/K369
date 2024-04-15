using UnityEngine;
using UnityEngine.UI;

public class MealLoader : MonoBehaviour
{
    public GameObject prefabToInstantiate;
    public Transform prefabParent;

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
}