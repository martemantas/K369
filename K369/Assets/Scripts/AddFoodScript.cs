using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;

public class AddFoodScript : MonoBehaviour
{
    public string FoodSearchScreenName = "FoodSearch screen";
    public TextMeshProUGUI foodName;
    public TextMeshProUGUI servingValueText;
    private float primary_G_value;
    private float primary_OZ_value;

    public TextMeshProUGUI nutritionLabel;
    public TextMeshProUGUI carbsText;
    public TextMeshProUGUI proteinsText;
    public TextMeshProUGUI fatText;
    public TextMeshProUGUI kcalText;
    private string carbsPrimaryValue;
    private string proteinsPrimaryValue;
    private string fatPrimaryValue;
    private string kcalPrimaryValue;
    private string servingPrimaryValue;
    private int selected_OG_or_OZ; // 0 - g, 1 - oz


    public TextMeshProUGUI countValueText;
    private int countValue = 1; // default value

    private List<string> mealOptions;
    private string selectedAddToOption;
    public TextMeshProUGUI selectedAddToOptionText;
    public TMP_Dropdown dropdownAddToSelection;
    private List<TMP_Dropdown.OptionData> options;

    private User userToAddFood; // user to add selected food to
    private DatabaseManager databaseManager;

    public GameObject ErrorMessage;

    void Start()
    {
        databaseManager = new DatabaseManager();
        selected_OG_or_OZ = 0;

        SetUser();                                       // child or parent
        SetFoodValues(userToAddFood);
        primary_OZ_value = Change_G_To_OZ(primary_G_value.ToString());
        mealOptions = GetUserMeals(userToAddFood);       // Adds only uncompleted meals
    }

    // Set user by its type - child or parent
    private void SetUser()
    {
        User currentUser = UserManager.Instance.CurrentUser;
        if (currentUser.userType == 2) // parent
        {
            userToAddFood = currentUser.child;
            userToAddFood.selectedFood = currentUser.selectedFood;
        }
        else
        {
            userToAddFood = currentUser;
        }
    }

    // Set food values to display and edit
    private void SetFoodValues(User user)
    {
        foodName.text = user.selectedFood.Name;
        kcalText.text = user.selectedFood.Calories.ToString();
        proteinsText.text = user.selectedFood.Protein.ToString();
        fatText.text = user.selectedFood.Fat.ToString();
        carbsText.text = user.selectedFood.Carbohydrates.ToString();
        servingValueText.text = user.selectedFood.Serving.ToString();
        primary_G_value = user.selectedFood.Serving;

        carbsPrimaryValue = carbsText.text;
        proteinsPrimaryValue = proteinsText.text;
        fatPrimaryValue = fatText.text;
        kcalPrimaryValue = kcalText.text;
        servingPrimaryValue = servingValueText.text;
    }

    // Make selection options
    private List<string> GetUserMeals(User user)
    {
        List<string> userMeals = new List<string> { "Select meal" };
        databaseManager.GetUserMeals(user.Id, (List<Meal> meals) =>
        {
            if (meals != null && meals.Count > 0)
            {
                foreach (Meal meal in meals)
                {
                    if (!meal.Completed)
                        userMeals.Add(meal.Name);
                }
                selectedAddToOption = mealOptions[0];   // default option is the first one
                AddOptionsToMealSelection();
                FormatNutritionLabel(countValue.ToString());
            }
            else
            {
                Debug.Log("User has no meals or meals are null.");
            }
        });
        return userMeals;
    }

    public void ExitButtonAction()
    {
        SceneManager.LoadScene(FoodSearchScreenName);
    }

    // Manage change of servings value
    public void HandleServingsValue(int val)
    {
        if (val == 0)
        {
            servingValueText.text = (primary_G_value * int.Parse(countValueText.text)).ToString();
            FormatNutritionLabel(countValueText.text);
            selected_OG_or_OZ = 0;
        }
        if (val == 1)
        {
            servingValueText.text = (Change_G_To_OZ(primary_G_value.ToString()) * int.Parse(countValueText.text)).ToString("0.00");
            FormatNutritionLabel(countValueText.text);
            selected_OG_or_OZ = 1;
        }
    }

    float Change_G_To_OZ(string value)
    {
        float g = float.Parse(value);
        float oz = g * 0.03527396f;
        return oz;
    }

    public void FormatNutritionLabel(string countValue)
    {
        string label = "Data for " + countValue + " pcs";
        nutritionLabel.text = label;
    }


    // Manage change of add to meal value
    public void HandleAddToValue(int val)
    {
        for (int i = 0; i < mealOptions.Count; i++)
        {
            if (i == val)
            {
                selectedAddToOption = mealOptions[i];
                break;
            }
        }
    }

    private void AddOptionsToMealSelection()
    {
        foreach (var meal in mealOptions)
        {
            dropdownAddToSelection.options.Add(new TMP_Dropdown.OptionData(meal));
        }
        dropdownAddToSelection.RefreshShownValue();
    }

    // Manage change of count value
    public void HandleCountValue(int val)
    {
        string servingsTemp = servingValueText.text;

        if (val == 0)
        {
            countValueText.text = 1.ToString();
        }
        if (val == 1)
        {
            countValueText.text = 2.ToString();
        }
        if (val == 2)
        {
            countValueText.text = 3.ToString();
        }
        if (val == 3)
        {
            countValueText.text = 4.ToString();
        }
        if (val == 4)
        {
            countValueText.text = 5.ToString();
        }
        servingValueText.text = servingsTemp;
        UpdateValues(countValueText.text);
        FormatNutritionLabel(countValueText.text);
    }

    public void AddToFavoritesButtonAction()
    {

    }

    public void ConfirmButtonAction()
    {
        Nutrient food = FormatFoodToAdd();
        if (selectedAddToOption.Equals("Select meal"))
        {
            ErrorMessage.SetActive(true);
        }
        else
        {
            UpdateMealValues(userToAddFood, selectedAddToOption, food);
            SceneManager.LoadScene(FoodSearchScreenName);
        }
    }

    // Add food to selected meal and update meal's nutritional and description values
    private void UpdateMealValues(User user, string mealName, Nutrient food)
    {
        string mealId = "";
        databaseManager.GetUserMeals(user.Id, (List<Meal> meals) =>
        {
            if (meals != null && meals.Count > 0)
            {
                foreach (Meal meal in meals)
                {
                    if (meal.Name.Equals(mealName))
                    {
                        mealId = meal.MealId;
                        break;
                    }
                }
                databaseManager.AddNewNutrientToMeal(user.Id, mealId, food); // Adds food to meal
                string foodInfo = $"{food.Name} ({food.Calories} kcal); "; // Updates meal values with added food values
                databaseManager.UpdateMealDescriptionAndValues(user.Id, mealId, foodInfo, food.Calories,
                                                               food.Protein, food.Fat, food.Carbohydrates);
            }
            else
            {
                Debug.Log("User has no meals or meals are null.");
            }
        });
    }

    private Nutrient FormatFoodToAdd()
    {
        string Id = userToAddFood.selectedFood.Id;
        string name = foodName.text;
        string date = "";
        int protein = int.Parse(proteinsText.text);
        int fats = int.Parse(fatText.text);
        int carbs = int.Parse(carbsText.text);
        int kcal = int.Parse(kcalText.text);
        float serving = float.Parse(servingValueText.text);
        int count = int.Parse(countValueText.text);
        string mealName = selectedAddToOption;
        Nutrient food = new Nutrient(Id, name, date, protein, fats, carbs, kcal, serving, count, mealName);
        return food;
    }

    // Update food values if changed
    public void UpdateValues(string count)
    {
        kcalText.text = (float.Parse(kcalPrimaryValue) * int.Parse(count)).ToString();
        fatText.text = (float.Parse(fatPrimaryValue) * int.Parse(count)).ToString();
        proteinsText.text = (float.Parse(proteinsPrimaryValue) * int.Parse(count)).ToString();
        carbsText.text = (float.Parse(carbsPrimaryValue) * int.Parse(count)).ToString();
        if (selected_OG_or_OZ == 0)
        {
            servingValueText.text = (float.Parse(servingPrimaryValue) * int.Parse(count)).ToString();
        }
        else
        {
            servingValueText.text = (Change_G_To_OZ(servingPrimaryValue) * int.Parse(count)).ToString("0.00");
        }
    }


}
