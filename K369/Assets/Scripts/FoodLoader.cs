using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class FoodLoader : MonoBehaviour
{
    public GameObject prefabToInstantiate;
    public Transform prefabParent;

    private List<Nutrient> FoodList;
    private FoodDatabaseManager databaseManager;
    public TMP_InputField searchInput;

    void Start()
    {
        databaseManager = new FoodDatabaseManager();
        databaseManager.SetFoodList();
        FoodList = databaseManager.GetFoodList();
        SpawnFood(FoodList);
    }

    public void SpawnFood(List<Nutrient> list)
    {
        foreach (Nutrient n in list)
        {
            CreateFoodInstance(n);
        }
    }

    private void CreateFoodInstance(Nutrient n)
    {
        GameObject foodInstance = Instantiate(prefabToInstantiate, prefabParent);
        FoodPrefabController controller = foodInstance.GetComponent<FoodPrefabController>();
        if (controller != null)
        {
            controller.Initialize(n.Id, n.Name, n.Calories.ToString(), n.Protein.ToString(),
                                  n.Fat.ToString(), n.Carbohydrates.ToString());
        }
    }

    public void OnSearchInputChanged()
    {
        ResetContent();
        var newList = FoodList;
        string searchString = searchInput.text;
        newList = newList.Where(n => n.Name.ToLower().Contains(searchString.ToLower())).ToList();
        SpawnFood(newList);
    }

    private void ResetContent()
    {
        foreach (Transform child in prefabParent)
        {
            Destroy(child.gameObject);
        }
    }


}
