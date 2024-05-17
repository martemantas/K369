using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class FoodDatabaseManager : MonoBehaviour
{
    private static List<Nutrient> FoodList;

    private void Start()
    {
        SetFoodList();
    }

    public void SetFoodList()
    {
        FoodList = FormatFoodList();
    }

    public List<Nutrient> GetFoodList()
    {
        return FoodList;
    }

    private List<Nutrient> FormatFoodList()
    {
        List<Nutrient> list = new List<Nutrient>();
        /*Nutrient n1 = new Nutrient("1", "Egg", "", 10, 10, 10, 30, 50, 1, "");
        Nutrient n2 = new Nutrient("2", "Bread", "", 20, 20, 20, 60, 80, 1, "");
        Nutrient n3 = new Nutrient("3", "Chicken", "", 30, 30, 30, 90, 150, 1, "");
        list.Add(n1);
        list.Add(n2);
        list.Add(n3);*/
        string jsonResponse = File.ReadAllText("Assets/Scripts/foundationDownload.json");
        list = ParseNutrientData(jsonResponse);
        return list;
    }

    public Nutrient GetFood(string name, string kcal, string fats, string proteins, string carbs)
    {
        Nutrient food = new Nutrient("", "Not found", "", 0, 0, 0, 0, 0, 0, "");
        List<Nutrient> foodList = GetFoodList();
        foreach (Nutrient n in foodList)
        {
            if (n.Name.Equals(name) && n.Calories.ToString().Equals(kcal) && n.Fat.ToString().Equals(fats)
                && n.Protein.ToString().Equals(proteins) && n.Carbohydrates.ToString().Equals(carbs))
            {
                food = n;
                break;
            }
        }
        return food;
    }
    
  private List<Nutrient> ParseNutrientData(string jsonResponse)
{
    var nutrients = new List<Nutrient>();
    var jsonObject = JObject.Parse(jsonResponse);

    if (jsonObject == null || jsonObject["FoundationFoods"] == null)
    {
        Debug.LogError("Invalid JSON response");
        return nutrients;
    }

    var foodItems = jsonObject["FoundationFoods"];

    foreach (var foodItem in foodItems)
    {
        if (foodItem == null || foodItem["description"] == null || foodItem["foodNutrients"] == null)
        {
            Debug.LogError("Invalid food item in JSON response");
            continue;
        }

        var nutrient = new Nutrient
        {
            Id = foodItem["fdcId"]?.ToString(),
            Name = foodItem["description"].ToString(),
            Date = "",
            Serving = 100,
            Count = 1,
            MealName = "" // You can change this based on your context
        };

        foreach (var nutrientInfo in foodItem["foodNutrients"])
        {
            if (nutrientInfo == null || nutrientInfo["nutrient"] == null || nutrientInfo["nutrient"]["id"] == null || nutrientInfo["amount"] == null)
            {
                Debug.LogError("Invalid nutrient info in food item");
                continue;
            }

            var nutrientId = (int)nutrientInfo["nutrient"]["id"];
            if (float.TryParse(nutrientInfo["amount"].ToString(), out var nutrientAmount))
            {
                switch (nutrientId)
                {
                    case 1003: // Protein
                        nutrient.Protein = (int)nutrientAmount;
                        break;
                    case 1004: // Total lipid (fat)
                        nutrient.Fat = (int)nutrientAmount;
                        break;
                    case 1005: // Carbohydrate, by difference
                        nutrient.Carbohydrates = (int)nutrientAmount;
                        break;
                    case 1008: // Energy
                        nutrient.Calories = (int)nutrientAmount;
                        break;
                }
            }
        }

        nutrients.Add(nutrient);
    }

    return nutrients;
}


}