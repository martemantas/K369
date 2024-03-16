using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreenScript : MonoBehaviour
{

    public string ScheduleScreenName = "Timetable screen";
    public string FoodPlanScreenName = "Nutrition screen";
    public string SettingsScreenName = "Settings";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Opens timetable screen
    public void ScheduleButtonAction()
    {
        SceneManager.LoadScene(ScheduleScreenName);
    }


    public void FoodPlanButtonAction()
    {
        SceneManager.LoadScene(FoodPlanScreenName);
    }

    public void ShopButtonAction()
    {

    }

    public void SettingsButtonAction()
    {
        SceneManager.LoadScene(SettingsScreenName);
    }

}
