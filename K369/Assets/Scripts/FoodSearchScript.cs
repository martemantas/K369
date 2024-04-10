using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

    void Start()
    {

    }

    public void BackButtonAction()
    {
        SceneManager.LoadScene(NutritionScreenName);
    }

    public void SelectedFoodButtonAction()
    {
        SceneManager.LoadScene(AddFoodScreenName);
    }

    // Need to implement
    public void OptionsButtonAction()
    {
        
    }

    // Need to implement
    public void RecentButtonAction()
    {
        
    }

    // Need to implement
    public void FavoritesButtonAction()
    {
        
    }

    // Need to implement
    public void MyFoodsButtonAction()
    {
        
    }



}
