using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class NutritionPage : MonoBehaviour
{
    public Image[] imagesPieChart;
    public float[] values;

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
    public string SettingsScreenName = "Settings screen";


    void Start()
    {
        SetValues();
    }

    // Set nutrition values 
    public void SetValues()
    {
        float total = 0;

        fatValue = float.Parse(fat.text);
        carbsValue = float.Parse(carbs.text);
        proteinsValue = float.Parse(proteins.text);
        kcalValue = float.Parse(kcal.text);

        kcalValue = fatValue + carbsValue + proteinsValue;
        kcal.text = kcalValue.ToString();

        values[0] = fatValue;
        values[1] = carbsValue;
        values[2] = proteinsValue;

        for (int i = 0; i < values.Length; i++)
        {
            total += FindPercentage(values, i);
            imagesPieChart[i].fillAmount = total;
        }
    }

    // Find percentages for pie chart ploting
    private float FindPercentage(float[] value, int index)
    {
        float total = 0;
        for(int i = 0; i < value.Length; i++)
        {
            total += value[i];
        }
        return value[index] / total;
    }

    public void ScheduleButtonAction()
    {
        SceneManager.LoadScene(ScheduleScreenName);
    }

    public void YouButtonAction()
    {
        SceneManager.LoadScene(YouScreenName);
    }

    // Need to implement
    public void SettingsButtonAction()
    {

    }

    // Need to implement
    public void AddMealButtonAction()
    {

    }

    // Need to implement
    public void EditButtonAction()
    {

    }

}
