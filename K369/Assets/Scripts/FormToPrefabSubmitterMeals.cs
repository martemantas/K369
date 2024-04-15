using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FormToPrefabSubmitterMeals : MonoBehaviour
{
    public TMP_InputField[] inputFields;
    public GameObject prefabToInstantiate;
    public Transform prefabParent;

    private int carbohydrates = 0;
    private int proteins = 0;
    private int fats = 0;
    private int kcal = 0;
    private int pointsToAdd = 10;

    public void OnSubmitButtonClick()
    {
        GameObject instantiatedPrefab = Instantiate(prefabToInstantiate, prefabParent ? prefabParent : null);
        User user = UserManager.Instance.CurrentUser;
        string mealId = Guid.NewGuid().ToString();
        user.Meals.Add(new Meal(mealId, inputFields[0].text, inputFields[1].text, carbohydrates, 
                                proteins, fats, false, "", "", "", pointsToAdd, kcal));
        if (user.userType != 0)
        {
            DatabaseManager.Instance.AddNewMeal(user.Id, mealId, inputFields[0].text, inputFields[1].text, carbohydrates,
                                proteins, fats, false, "", "", "", pointsToAdd, kcal);
        }
        MealPrefabController controller = instantiatedPrefab.GetComponent<MealPrefabController>();
        if (controller != null)
        {
            controller.Initialize(mealId, inputFields[0].text, inputFields[1].text, pointsToAdd);
        }
    }
}
