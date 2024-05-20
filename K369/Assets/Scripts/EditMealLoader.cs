using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    private bool isRemoveButtonActive = true;
    private string childID;
    private DatabaseManager databaseManager;

    public GameObject errorMessageForGuest;
    public GameObject editPlanScreen;
    User user;

    private void Start()
    {
        databaseManager = new DatabaseManager();
        ResetContent();
        Spawn();
    }

    public async void Spawn()
    {
        User currentUser = UserManager.Instance.CurrentUser;
        if (currentUser.userType == 2) // parent
        {
            childID = UserManager.Instance.GetSelectedChildToViewID();
            if (childID != null)
            {
                user = await FindUserChild();
                SpawnUserMeals(user);
            }
        }
        else
        {
            user = currentUser;
            SpawnUserMeals(user);
        }

    }

    public void SpawnUserMeals(User user)
    {
        ResetContent();

        if (user != null)
        {
            databaseManager.GetUserMeals(user.Id, (List<Meal> meals) =>
            {
                if (meals != null && meals.Count > 0)
                {
                    foreach (Meal meal in user.Meals)
                    {
                        GameObject mealInstance = Instantiate(prefabToInstantiate, prefabParent);
                        MealEditPrefabController controller = mealInstance.GetComponent<MealEditPrefabController>();
                        if (controller != null)
                        {
                            controller.Initialize(meal.MealId, meal.Name, meal.Description, meal.Points, isRemoveButtonActive);
                        }
                    }
                }
                else
                {
                    Debug.Log("User has no meals or meals are null.");
                }
            });
        }
        else
        {
            Debug.Log("User is null.");
        }
    }


    public void OnExitButton()
    {
        SceneManager.LoadScene(NutritionScreenName);
    }

    public void OnEditButton()
    {
        ResetContent();

        // if user is not guest
        if (user.userType != 0)
        {
            editPlanScreen.SetActive(true);
            Spawn();
        }
        if (user.userType == 0)
        {
            // show error message
            errorMessageForGuest.SetActive(true);
        }
    }

    // Delete prefabs
    private void ResetContent()
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

    public void SetChildID(string id)
    {
        childID = id;
    }

    public string GetChildID()
    {
        return childID;
    }

    public async Task<User> FindUserChild()
    {
        User user = await databaseManager.FindUserByChildID(childID);
        return user;

    }

}