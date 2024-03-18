using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class NutritionPage : MonoBehaviour
{
    public Image[] imagesPieChart;

    public TMP_Text fat;
    public TMP_Text carbs;
    public TMP_Text proteins;
    public TMP_Text kcal;

    public float fatValue;
    public float carbsValue;
    public float proteinsValue;
    public float kcalValue;

    public string ScheduleScreenName = "Timetable screen";
    public string YouScreenName = "Main screen";
    public string SettingsScreenName = "Settings";

    public GameObject scrollViewContent;
    public GameObject newMealCardButton;


    void Start()
    {
        SetValues();
    }

    // Counts nutrition values
    public void SetValues()
    {
        fatValue = float.Parse(fat.text);
        carbsValue = float.Parse(carbs.text);
        proteinsValue = float.Parse(proteins.text);
        kcalValue = float.Parse(kcal.text);

        kcalValue = fatValue + carbsValue + proteinsValue;
        kcal.text = kcalValue.ToString();

        float[] values = { fatValue, carbsValue, proteinsValue };
        PieChartCalculation(values);
    }

    public float GetKCalValue()
    {
        return kcalValue;
    }

    public float GetFatValue()
    {
        return fatValue;
    }

    public float GetCarbsValue()
    {
        return carbsValue;
    }

    public float GetProteinsValue()
    {
        return proteinsValue;
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

    // Adds new meal card. Does not save anything currently
    public void AddMealButtonAction()
    {
        GameObject newCard = (GameObject)Instantiate(newMealCardButton);
        newCard.transform.SetParent(scrollViewContent.transform);
        newCard.transform.localScale = new Vector3(1, 1, 1);
    } 

    public void SettingsButtonAction()
    {
        SceneManager.LoadScene(SettingsScreenName);
    }

    // Need to implement
    public void EditButtonAction()
    {

    }

}
