using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR;
using Unity.VisualScripting;

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

    public EditMealLoader loader;
    public ScrollRect scrollView;
    public Transform contentContainer;

    public void OnSubmitButtonClick()
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
                                proteins, fats, false, "", "", "", pointsToAdd, kcal));
        if (user.userType != 0)
        {
            DatabaseManager.Instance.AddNewMeal(user.Id, mealId, inputFields[0].text, inputFields[1].text, carbohydrates,
                                proteins, fats, false, "", "", "", pointsToAdd, kcal);
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



}

