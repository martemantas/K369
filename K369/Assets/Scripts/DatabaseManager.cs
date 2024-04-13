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
    public static DatabaseManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevents the UserManager from being destroyed when changing scenes
        }
        else
        {
            Destroy(gameObject); // Ensures there is only one instance of the UserManager
        }
    }

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
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
        User newUser = new User(userId, username, password, email, birthday, registrationDate, 0, 0, 0, 0, 0,1);
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
    
    public void UpdateUserDetails(string userId, string newUsername, string newEmail, string newBirthday)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(newUsername)) updates["Username"] = newUsername;
        if (!string.IsNullOrEmpty(newEmail)) updates["Email"] = newEmail;
        if (!string.IsNullOrEmpty(newBirthday)) updates["Birthday"] = newBirthday;

        UpdateUser(userId, updates);
    }

    public void UpdateUser(string userId, Dictionary<string, object> updates)
    {
        DatabaseReference userRef = databaseReference.Child("Users").Child(userId);
        userRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Error updating user: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("User updated successfully for userId: " + userId);
            }
        });
    }
    public void UpdateUserPoints(string userId, int newPoints)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["Points"] = newPoints;

        UpdateUser(userId, updates);
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

    public void AddNewTask(string userId, string taskId, string name, string description, string dateAdded, string dateCompleted, string dateExpire, int points, bool completed) {
        Task newTask = new Task(taskId, name, description, dateAdded, dateCompleted, dateExpire, points, completed);
        string json = JsonUtility.ToJson(newTask);
    
        Debug.Log($"Adding task to path: Users/{userId}/Tasks/{taskId} with data: {json}");

        databaseReference.Child("Users").Child(userId).Child("Tasks").Child(taskId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
            if (task.IsFaulted) {
                // Log the error
                foreach(var exception in task.Exception.Flatten().InnerExceptions) {
                    FirebaseException firebaseEx = exception as FirebaseException;
                    if(firebaseEx != null) {
                        Debug.LogError($"Error adding new task: {firebaseEx.ErrorCode} - {firebaseEx.Message}");
                    } else {
                        Debug.LogError("Error adding new task: " + exception.Message);
                    }
                }
            } else if (task.IsCompleted) {
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
    
    public void MarkTaskAsCompleted(string userId, string taskId)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["Completed"] = true;  
        UpdateTask(userId, taskId, updates);
    }
    public void UpdateTaskDetails(string userId, string taskId, string newName, string newDescription)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["Name"] = newName;
        updates["Description"] = newDescription;

        UpdateTask(userId, taskId, updates);
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
                            User foundUser = JsonUtility.FromJson<User>(childSnapshot.GetRawJsonValue());
                            callback(foundUser); 
                            return; 
                        }
                    }
                    else
                    {
                        Debug.Log("No user found with the specified email.");
                        callback(null); 
                    }
                }
            });
    }


public void GetUserByEmailAndPassword(string email, string password, Action<User> callback)
{
    FirebaseDatabase.DefaultInstance
        .GetReference("Users")
        .OrderByChild("Email")
        .EqualTo(email)
        .GetValueAsync().ContinueWithOnMainThread(task =>
        {
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

                        User foundUser = JsonUtility.FromJson<User>(childSnapshot.GetRawJsonValue());

                        if (foundUser.Password == password)
                        {
                            foundUser.Tasks.Clear();

                            DataSnapshot tasksSnapshot = childSnapshot.Child("Tasks");
                            foreach (DataSnapshot taskSnapshot in tasksSnapshot.Children)
                            {
                                Task _task = JsonUtility.FromJson<Task>(taskSnapshot.GetRawJsonValue());
                                foundUser.Tasks.Add(_task);
                            }

                            foundUser.Id = childSnapshot.Key;
                            callback(foundUser);
                            return;
                        }
                    }
                    Debug.Log("No user found with the specified email and password.");
                    callback(null); // No user found with the specified password
                }
                else
                {
                    Debug.Log("No user found with the specified email.");
                    callback(null); // No user found with the specified email
                }
            }
        });
}




}

[System.Serializable]
public class User
{
    public string Id;
    public string Username;
    public string Password;
    public string Email;
    public string Birthday;
    public string SaveData = "";
    public string RegistrationDate;
    public int todayCarbs = 0;
    public int todayProtein = 0;
    public int todayFat = 0;
    public int todayCalories = 0;
    public int Points = 0;
    public List<Task> Tasks = new List<Task>();
    public int userType = 0; // 0 - guest, 1 - user, 2 - parent
    
    public User(string id, string username, string password, string email, string birthday, string registrationDate, int todayCarbs, int todayProtein, int todayFat, int todayCalories, int points, int type)
    {
        Id = id;
        Username = username;
        Password = password; // Store passwords (not)securely
        Email = email;
        Birthday = birthday;
        SaveData = "";
        RegistrationDate = registrationDate;
        this.todayCarbs = todayCarbs;
        this.todayProtein = todayProtein;
        this.todayFat = todayFat;
        this.todayCalories = todayCalories;
        Points = points;
        userType = type;
    }
}

[System.Serializable]
public class Meal
{
    public string Name;
    public string Description;
    public int Carbohydrates;
    public int Protein;
    public int Fat;
    public bool Completed;
    public string DateAdded;
    public string DateCompleted;
    public string DateExpire;
    public int Points;
    public int Calories;

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
    public string TaskId;
    public string Name;
    public string Description;
    public string DateAdded;
    public string DateCompleted;
    public string DateExpire;
    public int Points;
    public bool Completed;

    public Task(string taskId, string name, string description, string dateAdded, string dateCompleted, string dateExpire, int points, bool completed)
    {
        TaskId = taskId;
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
    public string Date;
    public int Protein;
    public int Fat;
    public int Carbohydrates;
    public int Calories;

    public Nutrient(string date, int protein, int fat, int carbohydrates, int calories)
    {
        Date = date;
        Protein = protein;
        Fat = fat;
        Carbohydrates = carbohydrates;
        Calories = calories;
    }
}

