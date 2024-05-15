using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
        Nutrient n1 = new Nutrient("1", "Egg", "", 10, 10, 10, 30, 50, 1, "");
        Nutrient n2 = new Nutrient("2", "Bread", "", 20, 20, 20, 60, 80, 1, "");
        Nutrient n3 = new Nutrient("3", "Chicken", "", 30, 30, 30, 90, 150, 1, "");
        list.Add(n1);
        list.Add(n2);
        list.Add(n3);
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


}