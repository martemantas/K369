using UnityEngine;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;

public class MealEditPrefabController : MonoBehaviour
{
    public Text nameText;
    public Text descriptionText;
    public Button completeButton;
    public Button removeButton;
    public Image background;
    private string mealId;
    private int Points;
    private GameObject mealPrefab;


    public void Initialize(string id, string name, string description, int points, bool isRemoveButtonActive)
    {
        mealId = id;
        nameText.text = name;
        descriptionText.text = description;
        Points = points;
        if (isRemoveButtonActive)
        {
            completeButton.GameObject().SetActive(false);
            removeButton.GameObject().SetActive(true);
        }
        else
        {
            completeButton.GameObject().SetActive(true);
            removeButton.GameObject().SetActive(false);
        }
    }


    public void SetMealPrefab(GameObject prefab)
    {
        mealPrefab = prefab;
    }

  
    public void OnDeleteButton()
    {
        Debug.Log("Delete pressed");
    }
}