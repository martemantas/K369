using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class FoodSearchScript : MonoBehaviour
{

    public string NutritionScreenName = "Nutrition screen";
    public string AddFoodScreenName = "AddFood screen";
    public Button RecentButton;
    public Button FavoritesButton;
    public Button MyFoodsButton;

    private FoodDatabaseManager databaseManager;
    private List<Nutrient> FoodList;


    void Start()
    {
        InitializeValues();
    }

    private void InitializeValues()
    {
        databaseManager = new FoodDatabaseManager();
        databaseManager.SetFoodList();
        FoodList = databaseManager.GetFoodList();
    }

    public void BackButtonAction()
    {
        SceneManager.LoadScene(NutritionScreenName);
    }



}
