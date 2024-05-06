using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Unity.VisualScripting;
using System.Threading.Tasks;


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
            StartCoroutine(CheckAndRemoveCompletedItems("Tasks"));
            StartCoroutine(CheckAndRemoveCompletedItems("Meals"));

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


    public void AddNewUser(string userId, string username, string password, string email, string birthday, string registrationDate, int age, int height, int weight, string gender, string goals, int type)
    {
        User newUser = new User(userId, username, password, email, birthday, registrationDate, 0, 0, 0, 0, 0,type, age, height, weight, gender, goals);
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
    
    public void DeleteTask(string userId, string taskId)
    {
        Debug.Log($"Deleting task with taskId: {taskId} from user with userId: {userId}");

        databaseReference.Child("Users").Child(userId).Child("Tasks").Child(taskId).RemoveValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                foreach (var exception in task.Exception.Flatten().InnerExceptions)
                {
                    FirebaseException firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        Debug.LogError($"Error deleting task: {firebaseEx.ErrorCode} - {firebaseEx.Message}");
                    }
                    else
                    {
                        Debug.LogError("Error deleting task: " + exception.Message);
                    }
                }
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Task deleted successfully.");
            }
        });
    }

    public void UpdateTaskDetails(string userId, string taskId, string newName, string newDescription, string newDateAdded, string newDateCompleted, string newDateExpire, int newPoints, bool newCompleted)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>()
        {
            {"Name", newName},
            {"Description", newDescription},
            {"DateAdded", newDateAdded},
            {"DateCompleted", newDateCompleted},
            {"DateExpire", newDateExpire},
            {"Points", newPoints},
            {"Completed", newCompleted}
        };

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
    public void UpdateUserAge(string userId, int newAge)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["Age"] = newAge;

        UpdateUser(userId, updates);
    }
    public void UpdateUserGender(string userId, string newGender)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["Gender"] = newGender;

        UpdateUser(userId, updates);
    }
    public void UpdateUserHeight(string userId, int newHeight)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["Height"] = newHeight;

        UpdateUser(userId, updates);
    }
    public void UpdateUserWeight(string userId, int newWeight)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["Weight"] = newWeight;

        UpdateUser(userId, updates);
    }



    public void AddNewMeal(string userId, string mealId, string name, string description, int carbohydrates, int protein, int fat, bool completed, string dateAdded, string dateCompleted, string dateExpire, int points, int calories)
    {
        Meal newMeal = new Meal(mealId, name, description, carbohydrates, protein, fat, completed, dateAdded, dateCompleted, dateExpire, points, calories);
        string json = JsonUtility.ToJson(newMeal);

        Debug.Log($"Adding meal to path: Users/{userId}/Meals/{mealId} with data: {json}");

        databaseReference.Child("Users").Child(userId).Child("Meals").Child(mealId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                // Log the error
                foreach (var exception in task.Exception.Flatten().InnerExceptions)
                {
                    FirebaseException firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        Debug.LogError($"Error adding new meal: {firebaseEx.ErrorCode} - {firebaseEx.Message}");
                    }
                    else
                    {
                        Debug.LogError("Error adding new meal: " + exception.Message);
                    }
                }
            }
            else if (task.IsCompleted)
            {
                Debug.Log("New meal added successfully for user: " + userId);
            }
        });
    }

    public void DeleteMeal(string userId, string mealId)
    {
        Debug.Log($"Deleting meal with mealId: {mealId} from user with userId: {userId}");

        databaseReference.Child("Users").Child(userId).Child("Meals").Child(mealId).RemoveValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                foreach (var exception in task.Exception.Flatten().InnerExceptions)
                {
                    FirebaseException firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        Debug.LogError($"Error deleting meal: {firebaseEx.ErrorCode} - {firebaseEx.Message}");
                    }
                    else
                    {
                        Debug.LogError("Error deleting meal: " + exception.Message);
                    }
                }
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Meal deleted successfully.");
            }
        });
    }

    public void GetUserMeals(string userId, Action<List<Meal>> callback)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Users/" + userId + "/Meals")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || !task.IsCompleted)
                {
                    Debug.LogError("Error fetching user meals: " + task.Exception);
                    callback(null);
                }
                else
                {
                    DataSnapshot snapshot = task.Result;
                    List<Meal> meals = new List<Meal>();
                    foreach (DataSnapshot mealSnapshot in snapshot.Children)
                    {
                        Meal _meal = JsonUtility.FromJson<Meal>(mealSnapshot.GetRawJsonValue());
                        meals.Add(_meal);
                    }
                    callback(meals);
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
    private IEnumerator CheckAndRemoveCompletedItems(string itemType)
    {
        while (true)
        {
            yield return new WaitForSeconds(7 * 24 * 3600f); // Wait for a week (7 days * 24 hours * 3600 seconds)

            var userId = UserManager.Instance.CurrentUser.Id;

            if (string.IsNullOrEmpty(userId))
            {
                Debug.LogWarning("User ID is null or empty.");
                continue;
            }

            var itemQuery = databaseReference.Child("Users").Child(userId).Child(itemType).OrderByChild("Completed").EqualTo(true);
            var itemSnapshotTask = itemQuery.GetValueAsync();

            yield return new WaitUntil(() => itemSnapshotTask.IsCompleted);

            if (itemSnapshotTask.IsFaulted)
            {
                Debug.LogError($"Error fetching completed {itemType}: " + itemSnapshotTask.Exception);
            }
            else if (itemSnapshotTask.IsCompleted)
            {
                DataSnapshot itemSnapshot = itemSnapshotTask.Result;
                if (itemSnapshot != null && itemSnapshot.Exists)
                {
                    foreach (var item in itemSnapshot.Children)
                    {
                        string itemId = item.Key;
                        RemoveItemFromDatabase(userId, itemType, itemId);
                    }
                }
            }
        }
    }

    private void RemoveItemFromDatabase(string userId, string itemType, string itemId)
    {
        Debug.Log($"Removing {itemType} from database: " + itemId);

        DatabaseReference itemRef = databaseReference.Child("Users").Child(userId).Child(itemType).Child(itemId);
        itemRef.RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Error removing {itemType}: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log($"{itemType} removed successfully: " + itemId);

                if (itemType == "Tasks")
                {
                    UserManager.Instance.DeleteTask(itemId);
                    GameObject taskGameObject = FindTaskGameObject(itemId);
                    if (taskGameObject != null)
                    {
                        Destroy(taskGameObject);
                        Debug.Log("Task GameObject destroyed");
                    }
                    else
                    {
                        Debug.LogWarning("Task GameObject reference is null.");
                    }
                }
                else if (itemType == "Meals")
                {
                    UserManager.Instance.DeleteMeal(itemId);
                    GameObject mealGameObject = FindMealGameObject(itemId);
                    if (mealGameObject != null)
                    {
                        Destroy(mealGameObject);
                        Debug.Log("Meal GameObject destroyed");
                    }
                    else
                    {
                        Debug.LogWarning("Meal GameObject reference is null.");
                    }
                }
            }
        });
    }
    private GameObject FindMealGameObject(string mealId)
    {
        foreach (GameObject mealGameObject in GameObject.FindGameObjectsWithTag("Meal"))
        {

            MealPrefabController mealController = mealGameObject.GetComponent<MealPrefabController>();
            if (mealController != null && mealController.mealId == mealId)
            {
                return mealGameObject;
            }
            else
            {
                Debug.Log("No meals found");
            }
        }

        return null; 
    }
    private GameObject FindTaskGameObject(string taskId)
    {
        foreach (GameObject taskGameObject in GameObject.FindGameObjectsWithTag("Task"))
        {

            TaskPrefabController taskController = taskGameObject.GetComponent<TaskPrefabController>();
            if (taskController != null && taskController.taskId == taskId)
            {
                return taskGameObject;
            }
            else
            {
                Debug.Log("No tasks found");
            }
        }

        return null;
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

    public void MarkMealAsCompleted(string userId, string mealId)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["Completed"] = true;
        updates["DateCompleted"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        updates["pointsGiven"] = true;
        UpdateMeal(userId, mealId, updates);
    }
    public void UpdateMealDetails(string userId, string mealId, string newName, string newDescription)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["Name"] = newName;
        updates["Description"] = newDescription;

        UpdateMeal(userId, mealId, updates);
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
                            foundUser.Meals.Clear();
                            foundUser.children.Clear(); 

                            DataSnapshot tasksSnapshot = childSnapshot.Child("Tasks");
                            foreach (DataSnapshot taskSnapshot in tasksSnapshot.Children)
                            {
                                Task _task = JsonUtility.FromJson<Task>(taskSnapshot.GetRawJsonValue());
                                foundUser.Tasks.Add(_task);
                            }

                            DataSnapshot mealsSnapshot = childSnapshot.Child("Meals");
                            foreach (DataSnapshot mealSnapshot in mealsSnapshot.Children)
                            {
                                Meal _meal = JsonUtility.FromJson<Meal>(mealSnapshot.GetRawJsonValue());
                                foundUser.Meals.Add(_meal);
                            }

                            DataSnapshot childrenSnapshot = childSnapshot.Child("children");
                            if (childrenSnapshot.Exists)
                            {
                                foreach (DataSnapshot childIdSnapshot in childrenSnapshot.Children)
                                {
                                    int childId = int.Parse(childIdSnapshot.Value.ToString());
                                    foundUser.children.Add(childId);
                                }
                            }

                            foundUser.Id = childSnapshot.Key;
                            callback(foundUser);
                            return;
                        }
                    }
                    Debug.Log("No user found with the specified email and password.");
                    callback(null); 
                }
                else
                {
                    Debug.Log("No user found with the specified email.");
                    callback(null);
                }
            }
        });
}

    
    public void AddChildToUser(string userId, int childId)
    {
        DatabaseReference userChildrenRef = databaseReference.Child("Users").Child(userId).Child("children");
        userChildrenRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving children: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<int> children = new List<int>();
                if (snapshot.Exists && snapshot.ChildrenCount > 0)
                {
                    foreach (DataSnapshot childSnapshot in snapshot.Children)
                    {
                        int existingChildId = int.Parse(childSnapshot.Value.ToString());
                        children.Add(existingChildId);
                    }
                }
                children.Add(childId);

                userChildrenRef.SetValueAsync(children).ContinueWithOnMainThread(updateTask =>
                {
                    if (updateTask.IsFaulted)
                    {
                        Debug.LogError("Error updating children: " + updateTask.Exception);
                    }
                    else if (updateTask.IsCompleted)
                    {
                        Debug.Log("Child added successfully to user: " + userId);
                    }
                });
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
    public List<Meal> Meals = new List<Meal>();
    public int userType = 0; // 0 - guest, 1 - user, 2 - parent
    public int Age;
    public int Height;
    public int Weight;
    public string Gender;
    public string Goals;
    public List<int> children = new List<int>();
    public User(string id, string username, string password, string email, string birthday, string registrationDate, int todayCarbs, int todayProtein, int todayFat, int todayCalories, int points, int type, int age, int height, int weight, string gender, string goals)
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
        Age = age;
        Height = height;
        Weight = weight;
        Goals = goals;
        Gender = gender;
    }
}

[System.Serializable]
public class Meal
{
    public string MealId;
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
    public bool pointsGiven = false;

    public Meal(string mealId, string name, string description, int carbohydrates, int protein, int fat, bool completed, string dateAdded, string dateCompleted, string dateExpire, int points, int calories)
    {
        MealId = mealId;
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
    public bool pointsGiven = false;

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

