using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR;

public class FoodPrefabController : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text kcalText;
    public TMP_Text proteinsText;
    public TMP_Text fatText;
    public TMP_Text carbsText;
    public Button selectButton;
    public Image background;
    private string nutrientId;
    private string AddFoodScreenName = "AddFood screen";

    public void Initialize(string id, string name, string kcal, string proteins, string fats, string carbs)
    {
        nutrientId = id;
        nameText.text = name;
        kcalText.text = kcal;
        proteinsText.text = proteins;
        fatText.text = fats;
        carbsText.text = carbs;
    }

    public void OnSelectButton()
    {
        FoodDatabaseManager databaseManager = new FoodDatabaseManager();
        Nutrient food = databaseManager.GetFood(nameText.text, kcalText.text, fatText.text,
                                                proteinsText.text, carbsText.text);
        User currentUser = UserManager.Instance.CurrentUser;
        currentUser.selectedFood = food;    // set selected food which will be added
        SceneManager.LoadScene(AddFoodScreenName);
    }
}

