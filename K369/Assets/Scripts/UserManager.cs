using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Unity.VisualScripting;

public class UserManager : MonoBehaviour
{
    DatabaseReference databaseReference;
    public static UserManager Instance { get; private set; }
    public User CurrentUser { get; private set; }
    private int playerAge;
    private int playerHeight;
    private int playerWeight;
    private string playerGender;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoginUser(User user)
    {
        CurrentUser = user;
        SetPlayerAge(user.Age);
        SetPlayerGender(user.Gender);
        SetPlayerHeight(user.Height);
        SetPlayerWeight(user.Weight);
    }
    public void DeleteMeal(string mealId)
    {
        if (CurrentUser != null && CurrentUser.Meals != null)
        {
            Meal mealToDelete = CurrentUser.Meals.Find(m => m.MealId == mealId);
            if (mealToDelete != null)
            {
                CurrentUser.Meals.Remove(mealToDelete);
            }
        }
    }
    public void SetPlayerAge(int age)
    {
        playerAge = age;
    }

    public int GetPlayerAge()
    {
        return playerAge;
    }
    public void SetPlayerGender(string gender)
    {
        playerGender = gender;
    }

    public string GetPlayerGender()
    {
        return playerGender;
    }
    public void SetPlayerHeight(int height)
    {
        playerHeight = height;
    }

    public int GetPlayerHeight()
    {
        return playerHeight;
    }
    
    public void SetPlayerWeight(int weight)
    {
        playerWeight = weight;
    }

    public int GetPlayerWeight()
    {
        return playerWeight;
    }
}