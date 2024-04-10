using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.UI;

public class AddFoodScript : MonoBehaviour
{
    public string FoodSearchScreenName = "FoodSearch screen";
    public TextMeshProUGUI foodName;
    public TextMeshProUGUI servingValueText;
    private float primary_G_value;
    private float primary_OZ_value;
    private string G_value_text;
    private string OZ_value_text;

    public TextMeshProUGUI nutritionLabel;
    public TextMeshProUGUI carbsText;
    private float carbs;
    public TextMeshProUGUI proteinsText;
    private float proteins;
    public TextMeshProUGUI fatText;
    private float fat;
    public TextMeshProUGUI kcalText;
    private float kcal;

    public TextMeshProUGUI countValueText;
    private int countValue;

    private string[] mealOptions;
    private string selectedAddToOption;
    public TextMeshProUGUI selectedAddToOptionText;
    public TMP_Dropdown dropdownAddToSelection;
    private List<TMP_Dropdown.OptionData> options;

    void Start()
    {
        countValue = 1; // default value
        primary_G_value = int.Parse(servingValueText.text);
        G_value_text = "(" + primary_G_value + " g)";
        primary_OZ_value = Change_G_To_OZ();
        OZ_value_text = "(" + primary_OZ_value.ToString("0.00") + " oz)";

        mealOptions = new []{ "Lunch", "Breakfast", "Dinner", "Other" };
        selectedAddToOption = mealOptions[0]; // default option is the first one
        selectedAddToOption = selectedAddToOptionText.text;
        AddOptionsToMealSelection();
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
            servingValueText.text = primary_G_value.ToString();
            FormatNutritionLabel(G_value_text, countValueText.text);
        }
        if (val == 1)
        {
            servingValueText.text = primary_OZ_value.ToString("0.00");
            FormatNutritionLabel(OZ_value_text, countValueText.text);
        }
    }

    float Change_G_To_OZ()
    {
        float g = primary_G_value;
        float oz = g * 0.03527396f;
        return oz;
    }

    public void FormatNutritionLabel(string servingValueText, string countValue)
    {
        string label = "Data for " + countValue + " pcs " + servingValueText;
        nutritionLabel.text = label;
    }


    // Manage change of add to meal value
    public void HandleAddToValue(int val)
    {
        for (int i = 0; i < mealOptions.Length; i++)
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
        string newLabel;
        if (servingsTemp.Equals(primary_G_value.ToString()))
        {
            newLabel = G_value_text;
        }
        else
        {
            newLabel = OZ_value_text;
        }

        if (val == 0)
        {
            countValueText.text = 1.ToString();
            countValue = 1;
        }
        if (val == 1)
        {
            countValueText.text = 2.ToString();
            countValue = 2;
        }
        if (val == 2)
        {
            countValueText.text = 3.ToString();
            countValue = 3;
        }
        if (val == 3)
        {
            countValueText.text = 4.ToString();
            countValue = 4;
        }
        if (val == 4)
        {
            countValueText.text = 5.ToString();
            countValue = 5;
        }
        servingValueText.text = servingsTemp;
        FormatNutritionLabel(newLabel, countValueText.text);
    }

    // Need to implement
    public void AddToFavoritesButtonAction()
    {

    }

    // Need to implement
    public void ConfirmButtonAction()
    {

    }
}
