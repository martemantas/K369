using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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
    private DatabaseManager databaseManager;

    private string childID;
    private int dayOffset = 0;

    private void Start()
    {
        databaseManager = new DatabaseManager();
        Spawn();
    }

    public async void Spawn()
    {
        User currentUser = UserManager.Instance.CurrentUser;
        if (currentUser.userType == 2) // parent
        {
            childID = UserManager.Instance.GetSelectedChildToViewID();
            if (childID != null) // Needs better validation
            {
                User userChild = await FindUserChild();
                Debug.Log("Meal loader: spawning today's meals for user (childID): " + userChild.childID);
                SpawnUserMeals(userChild, 0);
            }
        }
        else
        {
            SpawnUserMeals(currentUser, 0);
        }

    }

    public void SpawnUserMeals(User user, int offset)
    {
        ResetContent();
        if (user != null)
        {
            databaseManager.GetUserMeals(user.Id, (List<Meal> meals) =>
            {
                if (meals != null && meals.Count > 0)
                {
                    foreach (Meal meal in meals)
                    {
                        DateTime mealDate = ConvertToDate(meal.DateAdded);
                        if (mealDate.Date.Day == DateTime.Now.Day + offset) // Spawn meals for the specified date
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
    
    public void ClickAction(bool isNextDay)
    {
        if (isNextDay)
        {
            dayOffset++;
        }
        else
        {
            dayOffset--;
        }

        SpawnUserMeals(UserManager.Instance.CurrentUser, dayOffset);
    }

    private DateTime ConvertToDate(string dateString)
    {
        DateTime result;
        if (DateTime.TryParseExact(dateString, "yyyy-MM-dd HH:mm", 
            CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        {
            return result;
        }
        else
        {
            Debug.Log("Cannot convert date to spawn today's meals.");
            return DateTime.Now;
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