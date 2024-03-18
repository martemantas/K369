using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScreenScript : MonoBehaviour
{

    public string ScheduleScreenName = "Timetable screen";
    public string FoodPlanScreenName = "Nutrition screen";
    public string SettingsScreenName = "Settings";

    // Status bars images
    public Image fatBarFull;
    public Image fatBarFilling;
    public Image proteinsBarFull;
    public Image proteinsBarFilling;
    public Image carbsBarFull;
    public Image carbsBarFilling;

    // Max values for each status bar
    public float maxFatValue = 100f;
    public float maxProteinsValue = 100f;
    public float maxCarbsValue = 100f;

    // Current values for each status bar
    public float currentFatValue = 50f;
    public float currentProteinsValue = 30f;
    public float currentCarbsValue = 20f;

    void Start()
    {
        SetBars();
    }

    // Get fat, proteins and carbs values from nutrition page
    void SetBars()
    {
        
    }

    void UpdateStatusBars()
    {
        // Update fatBar
        float fatFillAmount = Mathf.Clamp01(currentFatValue / maxFatValue);
        fatBarFilling.fillAmount = fatFillAmount;

        // Update proteinsBar
        float proteinsFillAmount = Mathf.Clamp01(currentProteinsValue / maxProteinsValue);
        proteinsBarFilling.fillAmount = proteinsFillAmount;

        // Update carbsBar
        float carbsFillAmount = Mathf.Clamp01(currentCarbsValue / maxCarbsValue);
        carbsBarFilling.fillAmount = carbsFillAmount;
    }

    // Example method to update values for all status bars
    public void UpdateValues(float newFatValue, float newProteinsValue, float newCarbsValue)
    {
        currentFatValue = newFatValue;
        currentProteinsValue = newProteinsValue;
        currentCarbsValue = newCarbsValue;

        UpdateStatusBars();
    }

    public void ScheduleButtonAction()
    {
        SceneManager.LoadScene(ScheduleScreenName);
    }


    public void FoodPlanButtonAction()
    {
        SceneManager.LoadScene(FoodPlanScreenName);
    }
    
    // Need to implement
    public void ShopButtonAction()
    {

    }

    public void SettingsButtonAction()
    {
        SceneManager.LoadScene(SettingsScreenName);
    }

}
