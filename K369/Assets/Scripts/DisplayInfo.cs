using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayInfo : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text levelText;

    public Image carbImage;
    public Image proteinImage;
    public Image fatImage;

    void Start()
    {
        string username = PlayerPrefs.GetString("Username");
        string points = PlayerPrefs.GetString("Points");

        if (usernameText.name == "Username text")
        {
            usernameText.text = username;
        }

        if (levelText.name == "Level text")
        {
            levelText.text = points;
        }

        displayCarbs();
        displayProtein();
        displayFat();
    }
    public void displayCarbs()
    {
        float totalCarbs = PlayerPrefs.GetFloat("Carbs");

        // Clamp the totalCarbs value between 0 and 1
        totalCarbs = Mathf.Clamp01(totalCarbs);

        //carbImage.fillAmount = totalCarbs;
    }
    public void displayProtein()
    {
        float totalProtein = PlayerPrefs.GetFloat("Protein");

        totalProtein = Mathf.Clamp01(totalProtein);

        proteinImage.fillAmount = totalProtein;
    }
    public void displayFat()
    {
        float totalFat = PlayerPrefs.GetFloat("Fat");

        totalFat = Mathf.Clamp01(totalFat);

        //fatImage.fillAmount = totalFat;
    }
}
