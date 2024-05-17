using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.VisualScripting;
using System;
using UnityEngine.XR;

public class NutritionPage : MonoBehaviour
{
    public Image[] imagesPieChart;

    public TMP_Text fat;
    public TMP_Text carbs;
    public TMP_Text proteins;
    public TMP_Text kcal;

    private User user;
    private string dateUpdated;
    private int age;
    private string gender;
    private string program;
    private int height;
    private int weight;

    private float requiredKCal;
    private float requiredCarbs;
    private float requiredFats;
    private float requiredProteins;

    public string ScheduleScreenName = "Timetable screen";
    public string YouScreenName = "Main screen";
    public string SettingsScreenName = "Settings";
    public string FoodSearchScreenName = "FoodSearch screen";

    void Start()
    {

        SetUser();
        SetUserValues();
        SetRequiredNutritionalValues();
        SetTodaysNutritionalValues();
        SetChartValues();
    }

    // Displays nutrition values
    public void SetChartValues()
    {
        string kcalBalance = user.todayCalories.ToString() + "/" + requiredKCal.ToString("0");
        string fatBalance = user.todayFat.ToString() + "/" + requiredFats.ToString("0");
        string proteinBalance = user.todayProtein.ToString() + "/" + requiredProteins.ToString("0");
        string carbsBalance = user.todayCarbs.ToString() + "/" + requiredCarbs.ToString("0");

        kcal.text = kcalBalance;
        fat.text = fatBalance;
        carbs.text = carbsBalance;
        proteins.text = proteinBalance;

        if (user.todayCalories != 0)
        {
            float[] values = { user.todayFat, user.todayCarbs, user.todayProtein };
            PieChartCalculation(values);
        }
    }

    // Calculates pie chart values
    public void PieChartCalculation(float[] values)
    {
        float total = values.Sum();
        float runningPercentage = 0f;
        for (int i = 0; i < values.Length; i++)
        {
            float percentage = values[i] / total;
            runningPercentage += percentage;
            imagesPieChart[i].fillAmount = runningPercentage;
        }
    }

    // Opens timetable screen
    public void ScheduleButtonAction()
    {
        SceneManager.LoadScene(ScheduleScreenName);
    }

    // Opens profile screen
    public void YouButtonAction()
    {
        SceneManager.LoadScene(YouScreenName);
    }

    public void SettingsButtonAction()
    {
        SceneManager.LoadScene(SettingsScreenName);
    }

    public void SearchFoodButtonAction()
    {
        SceneManager.LoadScene(FoodSearchScreenName);
    }

    private void SetUser()
    {
        User currentUser = UserManager.Instance.CurrentUser;
        if (currentUser.userType == 2) // parent
        {
            user = currentUser.child;
        }
        else
        {
            user = currentUser;
        }
    }

    private void SetUserValues()
    {
        if (user.userType == 0) // for guest, set default values
        {
            age = 20;
            program = "For fun";
            gender = "male";
        }
        if (gender == null)
        {
            gender = "male";
        }
        if (program == null)
        {
            program = "For fun";
        }
        if (age <= 0)
        {
            age = 20;
        }
        
        weight = user.Weight;
        height = user.Height;
        age = user.Age;
        program = user.Goals;
        gender = user.Gender;
        if (height <= 0 || weight <= 0) // set default values
        {
            // According to https://www.worlddata.info/average-bodyheight.php , lets say that average person
            // is 175cm height and 75kg weight
            height = 175;
            weight = 75;
        }

        DatabaseManager.Instance.UpdateUserHeight(user.Id, height);
        DatabaseManager.Instance.UpdateUserWeight(user.Id, weight);
        DatabaseManager.Instance.UpdateUserAge(user.Id, age);
        DatabaseManager.Instance.UpdateUserGender(user.Id, gender);

    }

    /// <summary>
    /// Set values by Harris-Benedict equation
    /// </summary>
    private void SetRequiredNutritionalValues()
    {
        float calories;
        if (gender.Equals("male"))
        {
            calories = (float)(88.362 + 13.397 * weight + 4.799 * height - 5.677 * age);
        }
        else
        {
            calories = (float)(447.593 + 9.247 * weight + 3.098 * height - 4.330 * age);
        }

        // select goal
        float multiplier = 1.000F;
        switch (program)
        {
            case "For fun":
                multiplier = 1.2F;
                break;
            case "Health":
                multiplier = 1.55F;
                break;
            case "Muscle":
                multiplier = 1.725F;
                break;
            default:
                break;
        }
        calories *= multiplier;
        calories = (float)Math.Round(calories, 2);
        user.requiredCalories = calories;
        requiredKCal = calories;

        // 30 percent of calories for proteins and fat, 40 percent for carbs
        float _03 = (float)Math.Round(calories * 0.3F, 2);
        float _04 = (float)Math.Round(calories * 0.4F, 2);
        user.requiredProtein = _03;
        requiredProteins = _03;
        user.requiredFat = _03;
        requiredFats = _03;
        user.requiredCarbs = _04;
        requiredCarbs = _04;

        DatabaseManager.Instance.UpdateUserRequiredNutritionalValues(user.Id, requiredKCal, requiredProteins, requiredFats, requiredCarbs);
    }

    private void SetTodaysNutritionalValues()
    {
        DateTime dateNow = DateTime.Now;
        dateUpdated = dateNow.ToString("yyyy-MM-dd");

        // If today is a new day compared to the last update, reset the values
        if (!user.nutritionalValuesUpdated.Equals(dateUpdated) || user.nutritionalValuesUpdated.Equals(""))
        {
            user.nutritionalValuesUpdated = dateUpdated;
            user.todayProtein = 0;
            user.todayCarbs = 0;
            user.todayCalories = 0;
            user.todayFat = 0;
            DatabaseManager.Instance.UpdateUserNutritionalValues(user.Id, 0, 0, 0, 0);
            DatabaseManager.Instance.UpdateUserNutritionUpdateTime(user.Id, user.nutritionalValuesUpdated);
        }
    }

    //private void SetTodaysNutritionalValues()
    //{
    //    //dateUpdated = DateTime.Now;
    //    //if (user.nutritionalValuesUpdated.Length == 0)
    //    //{
    //    //    Debug.Log("values are updated first time.");
    //    //    user.nutritionalValuesUpdated = dateUpdated.ToString("yyyy-MM-dd");
    //    //}
    //    //// if date was never updated or today is the new day, update date and reset values
    //    //else if (DateTime.Parse(user.nutritionalValuesUpdated).Day != dateUpdated.Day)
    //    //{
    //    //    user.nutritionalValuesUpdated = dateUpdated.ToString("yyyy-MM-dd");
    //    //    user.todayProtein = 0;
    //    //    user.todayCarbs = 0;
    //    //    user.todayCalories = 0;
    //    //    user.todayFat = 0;
    //    //    DatabaseManager.Instance.UpdateUserNutritionalValues(user.Id, 0, 0, 0, 0);
    //    //}

    //    dateUpdated = DateTime.Now;

    //    if (string.IsNullOrEmpty(user.nutritionalValuesUpdated))
    //    {
    //        Debug.Log("Values are updated for the first time.");
    //        user.nutritionalValuesUpdated = dateUpdated.ToString("yyyy-MM-dd");
    //    }
    //    else
    //    {
    //        DateTime nutritionalValuesDate;
    //        bool parseSuccess = DateTime.TryParse(user.nutritionalValuesUpdated, out nutritionalValuesDate);

    //        if (!parseSuccess)
    //        {
    //            Debug.LogError("Failed to parse the date from user.nutritionalValuesUpdated.");
    //            nutritionalValuesDate = dateUpdated;
    //            user.nutritionalValuesUpdated = dateUpdated.ToString("yyyy-MM-dd");
    //        }

    //        Debug.Log("nutritionalValuesDate day: " + nutritionalValuesDate.Day);
    //        Debug.Log("date upadted day: " + dateUpdated.Day);

    //        // If today is a new day compared to the last update, reset the values
    //        if (nutritionalValuesDate.Day != dateUpdated.Day)
    //        {
    //            Debug.Log("New day detected. Resetting nutritional values.");
    //            //user.nutritionalValuesUpdated = dateUpdated.ToString("yyyy-MM-dd");
    //            //user.todayProtein = 0;
    //            //user.todayCarbs = 0;
    //            //user.todayCalories = 0;
    //            //user.todayFat = 0;
    //            //DatabaseManager.Instance.UpdateUserNutritionalValues(user.Id, 0, 0, 0, 0);
    //        }
    //    }

    //    Action<User> userCallback = (updatedUser) =>
    //    {
    //        fatValue = updatedUser.todayFat;
    //        proteinsValue = updatedUser.todayProtein;
    //        carbsValue = updatedUser.todayCarbs;
    //        kcalValue = updatedUser.todayCalories;
    //    };
    //    DatabaseManager.Instance.GetUserByEmail(user.Email, userCallback);
    //    Debug.Log("upadted proteins: " + proteinsValue);


    //   // DatabaseManager.Instance.UpdateUserNutritionalValues(user.Id, kcalValue, proteinsValue, fatValue, carbsValue);
    //    DatabaseManager.Instance.UpdateUserNutritionUpdateTime(user.Id, user.nutritionalValuesUpdated);
    //}


}
