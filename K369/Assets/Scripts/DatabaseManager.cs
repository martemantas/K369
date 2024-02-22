using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class DatabaseManager : MonoBehaviour
{
    DatabaseReference databaseReference;

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        //AddNewUser("newId", "user155", "password", "asd@emaio.com", "2021-10-01", "2021-10-01");
        ExampleGetUser();
    }
    
    //Firebase is asynchronous, so we use callbacks to handle the results
    //This is an example of how to use the GetUserByEmail method
    void ExampleGetUser()
    {
        GetUserByEmail("asd@emaio.com", (User user) => {
            if (user != null)
            {
                Debug.Log($"User found: {user.Username}");
                // Do something with the user object
            }
            else
            {
                Debug.Log("User not found.");
            }
        });
    }


    public void AddNewUser(string userId, string username, string password, string email, string birthday, string registrationDate)
    {
        User newUser = new User(username, password, email, birthday, registrationDate);
        string json = JsonUtility.ToJson(newUser);

        databaseReference.Child("Users").Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Error adding new user: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("New user added successfully");
            }
        });
    }


    
    public void AddNewMeal(string userId, string mealId, string name, string description, int carbohydrates, int protein, int fat, bool completed, string dateAdded, string dateCompleted, string dateExpire, int points, int calories)
    {
        Meal newMeal = new Meal(name, description, carbohydrates, protein, fat, completed, dateAdded, dateCompleted, dateExpire, points, calories);
        string json = JsonUtility.ToJson(newMeal);

        databaseReference.Child("Users").Child(userId).Child("Meals").Child(mealId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Error adding new meal: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("New meal added successfully for user: " + userId);
            }
        });
    }

    public void AddNewTask(string userId, string taskId, string name, string description, string dateAdded, string dateCompleted, string dateExpire, int points, bool completed)
    {
        Task newTask = new Task(name, description, dateAdded, dateCompleted, dateExpire, points, completed);
        string json = JsonUtility.ToJson(newTask);

        databaseReference.Child("Users").Child(userId).Child("Tasks").Child(taskId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Error adding new task: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("New task added successfully for user: " + userId);
            }
        });
    }

    public void AddNewNutrientRecord(string userId, string nutrientId, string date, int protein, int fat, int carbohydrates, int calories)
    {
        Nutrient newNutrient = new Nutrient(date, protein, fat, carbohydrates, calories);
        string json = JsonUtility.ToJson(newNutrient);

        databaseReference.Child("Users").Child(userId).Child("Nutrients").Child(nutrientId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Error adding new nutrient record: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("New nutrient record added successfully for user: " + userId);
            }
        });
    }

    
    public void UpdateMeal(string userId, string mealId, Dictionary<string, object> updates)
    {
        DatabaseReference mealRef = databaseReference.Child("Users").Child(userId).Child("Meals").Child(mealId);
        mealRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Error updating meal: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Meal updated successfully for user: " + userId);
            }
        });
    }
    
    public void UpdateNutrient(string userId, string nutrientId, Dictionary<string, object> updates)
    {
        DatabaseReference nutrientRef = databaseReference.Child("Users").Child(userId).Child("Nutrients").Child(nutrientId);
        nutrientRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Error updating nutrient record: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Nutrient record updated successfully for user: " + userId);
            }
        });
    }
    
    public void UpdateTask(string userId, string taskId, Dictionary<string, object> updates)
    {
        DatabaseReference taskRef = databaseReference.Child("Users").Child(userId).Child("Tasks").Child(taskId);
        taskRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Error updating task: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Task updated successfully for user: " + userId);
            }
        });
    }

    public void GetUserByEmail(string email, Action<User> callback)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .OrderByChild("Email")
            .EqualTo(email)
            .GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsFaulted || !task.IsCompleted)
                {
                    Debug.LogError("Error fetching user: " + task.Exception);
                    callback(null); 
                }
                else
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists && snapshot.ChildrenCount > 0)
                    {
                        foreach (var childSnapshot in snapshot.Children)
                        {
                            // Deserialize the found user snapshot to a User object
                            User foundUser = JsonUtility.FromJson<User>(childSnapshot.GetRawJsonValue());
                            callback(foundUser); // Return the found user via callback
                            return; // Assuming only one user will match
                        }
                    }
                    else
                    {
                        Debug.Log("No user found with the specified email.");
                        callback(null); // No user found
                    }
                }
            });
    }
}

[System.Serializable]
public class User
{
    public string Username;
    public string Password;
    public string Email;
    public string Birthday;
    public string SaveData = "";
    public string RegistrationDate;

    public User(string username, string password, string email, string birthday, string registrationDate)
    {
        Username = username;
        Password = password; // Store passwords (not)securely
        Email = email;
        Birthday = birthday;
        SaveData = "";
        RegistrationDate = registrationDate;
    }
}

[System.Serializable]
public class Meal
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Carbohydrates { get; set; }
    public int Protein { get; set; }
    public int Fat { get; set; }
    public bool Completed { get; set; }
    public string DateAdded { get; set; }
    public string DateCompleted { get; set; }
    public string DateExpire { get; set; }
    public int Points { get; set; }
    public int Calories { get; set; }

    public Meal(string name, string description, int carbohydrates, int protein, int fat, bool completed, string dateAdded, string dateCompleted, string dateExpire, int points, int calories)
    {
        Name = name;
        Description = description;
        Carbohydrates = carbohydrates;
        Protein = protein;
        Fat = fat;
        Completed = completed;
        DateAdded = dateAdded;
        DateCompleted = dateCompleted;
        DateExpire = dateExpire;
        Points = points;
        Calories = calories;
    }
}

[System.Serializable]
public class Task
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string DateAdded { get; set; }
    public string DateCompleted { get; set; }
    public string DateExpire { get; set; }
    public int Points { get; set; }
    public bool Completed { get; set; }

    public Task(string name, string description, string dateAdded, string dateCompleted, string dateExpire, int points, bool completed)
    {
        Name = name;
        Description = description;
        DateAdded = dateAdded;
        DateCompleted = dateCompleted;
        DateExpire = dateExpire;
        Points = points;
        Completed = completed;
    }
}

[System.Serializable]
public class Nutrient
{
    public string Date { get; set; }
    public int Protein { get; set; }
    public int Fat { get; set; }
    public int Carbohydrates { get; set; }
    public int Calories { get; set; }

    public Nutrient(string date, int protein, int fat, int carbohydrates, int calories)
    {
        Date = date;
        Protein = protein;
        Fat = fat;
        Carbohydrates = carbohydrates;
        Calories = calories;
    }
}

