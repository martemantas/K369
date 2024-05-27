using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Unity.VisualScripting;
using System.Threading.Tasks;
using Random = System.Random;

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
    
    public void AddNewUser(string userId, string username, string password, string email, string birthday, string registrationDate, int age, int height, int weight, string gender, string goals, int type)
    {
        UserManager userManager = UserManager.Instance;
        int userType = userManager.GetPlayerType();
        string childID = GenerateChildID(userType);
        userManager.SetPlayerChildID(childID);

        User newUser = new User(userId, username, password, email, birthday, registrationDate, 0, 0, 0, 0, 0, userType, age,
                                height, weight, gender, goals, childID);
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

    /// <summary>
    /// Generates child id by its username and current date
    /// </summary>
    /// <param name="userType"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    private static string GenerateChildID(int userType) 
    {
        if (userType != 1)
        {
            return "";
        }
        DateTime currentDate = DateTime.Now;
        Random random = new Random();
        string part0 = random.Next(0, 9).ToString();
        string part1 = currentDate.Minute.ToString("00");
        string part2 = currentDate.Second.ToString("00");
        string part3 = random.Next(0, 9).ToString();
        int childID = int.Parse(part0 + part1 + part2 + part3);
        return childID.ToString();
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

    public void UpdateUserNutritionalValues(string userId, float newKcalToday, float newProteinsToday, 
                                            float newFatsToday, float newCarbsToday)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["todayCalories"] = newKcalToday;
        updates["todayCarbs"] = newCarbsToday;
        updates["todayFat"] = newFatsToday;
        updates["todayProtein"] = newProteinsToday;

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
    public void UpdateUserGoals(string userId, int newGoal)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["Goals"] = newGoal;

        UpdateUser(userId, updates);
    }


    public void UpdateUserNutritionUpdateTime(string userId, string newDate)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["nutritionalValuesUpdated"] = newDate;

        UpdateUser(userId, updates);
    }

    public void UpdateUserRequiredNutritionalValues(string userId, float newKcalToday, float newProteinsToday,
                                            float newFatsToday, float newCarbsToday)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["requiredCalories"] = newKcalToday;
        updates["requiredCarbs"] = newCarbsToday;
        updates["requiredFat"] = newFatsToday;
        updates["requiredProtein"] = newProteinsToday;

        UpdateUser(userId, updates);
    }

    public void AddNewNutrientToMeal(string userId, string mealId, Nutrient food)
    {
        Debug.Log("Recieved values: userid " + userId + ", mealid " + mealId + ", foodId " + food.Id);
        string json = JsonUtility.ToJson(food);

        string path = "Users/" + userId + "/Meals/" + mealId + "/Nutrients/" + food.Id;

        // Update the path to include the meal ID
        //DatabaseReference nutrientRef = databaseReference.Child(path);
        DatabaseReference nutrientRef = FirebaseDatabase.DefaultInstance.RootReference.Child(path);

        nutrientRef.SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Error adding new nutrient record: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("New nutrient record added successfully for user: " + userId + " in meal: " + mealId);
            }
        });
    }

    // Updates meal (adds values of added food information to meal)
    public void UpdateMealDescriptionAndValues(string userId, string mealId, string descriptionAddition, int kcal, int proteins, int fats, int carbs)
    {

        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        databaseReference.Child("Users").Child(userId).Child("Meals").Child(mealId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving meal details: " + task.Exception);
                return;
            }

            DataSnapshot mealSnapshot = task.Result;
            if (mealSnapshot.Exists)
            {
                // Retrieve individual field values
                string currentDescription = mealSnapshot.Child("Description").Value.ToString();
                int currentCalories = int.Parse(mealSnapshot.Child("Calories").Value.ToString());
                int currentProteins = int.Parse(mealSnapshot.Child("Protein").Value.ToString());
                int currentFats = int.Parse(mealSnapshot.Child("Fat").Value.ToString());
                int currentCarbs = int.Parse(mealSnapshot.Child("Carbohydrates").Value.ToString());

                // Update values based on the inputs
                string updatedDescription = currentDescription + descriptionAddition;
                int updatedCalories = currentCalories + kcal;
                int updatedProteins = currentProteins + proteins;
                int updatedFats = currentFats + fats;
                int updatedCarbs = currentCarbs + carbs;

                // Create a dictionary with the updated values
                Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { "Description", updatedDescription },
                    { "Calories", updatedCalories },
                    { "Protein", updatedProteins },
                    { "Fat", updatedFats },
                    { "Carbohydrates", updatedCarbs }
                };

                // Update the meal with the updated values
                DatabaseReference mealRef = databaseReference.Child("Users").Child(userId).Child("Meals").Child(mealId);
                mealRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(updateTask =>
                {
                    if (updateTask.IsFaulted)
                    {
                        Debug.LogError("Error updating meal: " + updateTask.Exception);
                    }
                    else if (updateTask.IsCompleted)
                    {
                        Debug.Log("Meal updated successfully for user: " + userId);
                    }
                });
            }
            else
            {
                Debug.LogError("Meal not found.");
            }
        });
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
    public void AddNewMealForChild(string childId, string mealId, string name, string description, int carbohydrates, int protein, int fat, bool completed, string dateAdded, string dateCompleted, string dateExpire, int points, int calories)
    {
        DatabaseReference usersRef = databaseReference.Child("Users");
        usersRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching users: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            // Iterate through the children to find the user with the matching child ID
            foreach (DataSnapshot userSnapshot in snapshot.Children)
            {
                IDictionary<string, object> userData = (IDictionary<string, object>)userSnapshot.Value;
                // Check if the user data contains a field named "ChildId" and its value matches the provided childId
                if (userData != null && userData.ContainsKey("childID") && userData["childID"].ToString() == childId)
                {
                    string userId = userData["Id"].ToString();
                    Debug.Log($"Found user with child ID {childId}. User name: {userId}");
                    AddNewMeal(userId, mealId, name, description, carbohydrates, protein, fat, completed, dateAdded, dateCompleted, dateExpire, points, calories);
                    return;
                }
            }

            Debug.LogWarning("No user found with child ID: " + childId);
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

    public void GetMealById(string userId, string mealId, Action<Meal> callback)
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
                    foreach (DataSnapshot mealSnapshot in snapshot.Children)
                    {
                        Meal _meal = JsonUtility.FromJson<Meal>(mealSnapshot.GetRawJsonValue());
                        if (_meal.MealId == mealId)
                        {
                            callback(_meal);
                            return; // Exit the method once the meal is found
                        }
                    }
                    // If the meal with the specified ID is not found
                    callback(null);
                }
            });
    }

    public void GetUserTasks(string userId, Action<List<Task>> callback)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Users/" + userId + "/Tasks")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || !task.IsCompleted)
                {
                    Debug.LogError("Error fetching user tasks: " + task.Exception);
                    callback(null);
                }
                else
                {
                    DataSnapshot snapshot = task.Result;
                    List<Task> tasks = new List<Task>();
                    foreach (DataSnapshot taskSnapshot in snapshot.Children)
                    {
                        Task _task = JsonUtility.FromJson<Task>(taskSnapshot.GetRawJsonValue());
                        tasks.Add(_task);
                    }
                    callback(tasks);
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
    public void AddNewTaskForChild(string childId, string taskId, string name, string description, string dateAdded, string dateCompleted, string dateExpire, int points, bool completed)
    {
        DatabaseReference usersRef = databaseReference.Child("Users");
        usersRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching users: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            // Iterate through the children to find the user with the matching child ID
            foreach (DataSnapshot userSnapshot in snapshot.Children)
            {
                IDictionary<string, object> userData = (IDictionary<string, object>)userSnapshot.Value;

                // Check if the user data contains a field named "ChildId" and its value matches the provided childId
                if (userData != null && userData.ContainsKey("childID") && userData["childID"].ToString() == childId)
                {
                    string userId = userData["Id"].ToString();
                    Debug.Log($"Found user with child ID {childId}. User name: {userId}");
                    AddNewTask(userId, taskId, name, description, dateAdded, dateCompleted, dateExpire, points, completed);
                    return;
                }
            }

            Debug.LogWarning("No user found with child ID: " + childId);
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

    public Task<User> FindUserByChildID(string id)
    {
        TaskCompletionSource<User> tcs = new TaskCompletionSource<User>();

        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .OrderByChild("childID")
            .EqualTo(id)
            .GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsFaulted || !task.IsCompleted)
                {
                    Debug.LogError("Error fetching user: " + task.Exception);
                    tcs.SetResult(null);
                }
                else
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists && snapshot.ChildrenCount > 0)
                    {
                        foreach (var childSnapshot in snapshot.Children)
                        {
                            User foundUser = JsonUtility.FromJson<User>(childSnapshot.GetRawJsonValue());
                            tcs.SetResult(foundUser);
                            return;
                        }
                    }
                    else
                    {
                        Debug.Log("No user found with the specified childID.");
                        tcs.SetResult(null);
                    }
                }
            });

        return tcs.Task;
    }
    public void RemoveChildFromParent(string parentId, string childId, Action<bool> callback)
    {
        DatabaseReference parentRef = FirebaseDatabase.DefaultInstance.GetReference("Users").Child(parentId).Child("children");

        parentRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || !task.IsCompleted)
            {
                Debug.LogError("Error fetching parent's children: " + task.Exception);
                callback(false);
            }
            else
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    List<string> updatedChildren = new List<string>();

                    foreach (DataSnapshot childSnapshot in snapshot.Children)
                    {
                        // Get the child ID from the snapshot key
                        string childKey = childSnapshot.Key;

                        // Add the child ID to the updated list if it's not the one being removed
                        if (childKey != childId)
                        {
                            updatedChildren.Add(childKey);
                        }
                    }

                    // Update the parent's children list in the database
                    parentRef.SetValueAsync(updatedChildren).ContinueWithOnMainThread(setTask =>
                    {
                        if (setTask.IsFaulted)
                        {
                            Debug.LogError("Error updating parent's children list: " + setTask.Exception);
                            callback(false);
                        }
                        else
                        {
                            callback(true);
                        }
                    });
                }
                else
                {
                    Debug.Log("Parent has no children or children list does not exist.");
                    callback(false);
                }
            }
        });
    }


    public void UpdateChildName(string childId, string newName, Action<bool> callback)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .OrderByChild("userType")
            .EqualTo(2) // Assuming 2 indicates a parent user type
            .GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsFaulted || !task.IsCompleted)
                {
                    Debug.LogError("Error fetching users: " + task.Exception);
                    callback(false);
                }
                else
                {
                    DataSnapshot snapshot = task.Result;
                    bool found = false;

                    foreach (DataSnapshot parentSnapshot in snapshot.Children)
                    {
                        if (parentSnapshot.Child("children").Exists)
                        {
                            foreach (DataSnapshot childSnapshot in parentSnapshot.Child("children").Children)
                            {
                                if (childSnapshot.Key == childId)
                                {
                                    string parentId = parentSnapshot.Key;
                                    string childKey = childSnapshot.Key;

                                    // Update the nickname field for the specific child entry
                                    databaseReference.Child("Users").Child(parentId).Child("children").Child(childKey).Child("nickname").SetValueAsync(newName).ContinueWithOnMainThread(updateTask => {
                                        if (updateTask.IsFaulted)
                                        {
                                            Debug.LogError("Error updating child name: " + updateTask.Exception);
                                            callback(false);
                                        }
                                        else
                                        {
                                            ChildManager.AddChildNickname(int.Parse(childId), newName);
                                            callback(true);
                                        }
                                    });

                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (found) break;
                    }

                    if (!found)
                    {
                        Debug.LogError("Child ID not found.");
                        callback(false);
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
                                foreach (DataSnapshot singleChildSnapshot in childrenSnapshot.Children)
                                {
                                    int childId = int.Parse(singleChildSnapshot.Key);
                                    string nickname = singleChildSnapshot.Child("nickname").Value.ToString();
                                    foundUser.children.Add(childId);
                                    ChildManager.AddChildNickname(childId, nickname);
                                }

                                DataSnapshot boughtItemsSnapshot = childSnapshot.Child("boughtItems");
                                if (boughtItemsSnapshot.Exists)
                                {
                                    foreach (DataSnapshot BoughtItemIdSnapshot in boughtItemsSnapshot.Children)
                                    {
                                        int childId = int.Parse(BoughtItemIdSnapshot.Value.ToString());
                                        foundUser.boughtItems.Add(childId);
                                    }
                                }
                            
                                DataSnapshot equippedItemsSnapshot = childSnapshot.Child("equippedItems");
                                if (equippedItemsSnapshot.Exists)
                                {
                                    foreach (DataSnapshot EquippedItemIdSnapshot in equippedItemsSnapshot.Children)
                                    {
                                        int childId = int.Parse(EquippedItemIdSnapshot.Value.ToString());
                                        foundUser.equipped.Add(childId);
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
                Dictionary<string, object> children = new Dictionary<string, object>();
                if (snapshot.Exists && snapshot.ChildrenCount > 0)
                {
                    foreach (DataSnapshot childSnapshot in snapshot.Children)
                    {
                        string existingChildId = childSnapshot.Key;
                        object childData = childSnapshot.Value;
                        children.Add(existingChildId, childData);
                    }
                }

                string newChildKey = childId.ToString();
                Dictionary<string, object> newChildData = new Dictionary<string, object>
                {
                    { "childID", childId },
                    { "nickname", "" }
                };

                children.Add(newChildKey, newChildData);
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
    
    public void BuyItemForUser(string userId, int itemId)
    {
        DatabaseReference userItemsRef = databaseReference.Child("Users").Child(userId).Child("boughtItems");
        userItemsRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving bought items: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<int> items = new List<int>();
                if (snapshot.Exists && snapshot.ChildrenCount > 0)
                {
                    foreach (DataSnapshot childSnapshot in snapshot.Children)
                    {
                        int existingBoughtItemId = int.Parse(childSnapshot.Value.ToString());
                        items.Add(existingBoughtItemId);
                    }
                }
                items.Add(itemId);

                userItemsRef.SetValueAsync(items).ContinueWithOnMainThread(updateTask =>
                {
                    if (updateTask.IsFaulted)
                    {
                        Debug.LogError("Error updating bought items: " + updateTask.Exception);
                    }
                    else if (updateTask.IsCompleted)
                    {
                        Debug.Log("Bought item added successfully to user: " + userId);
                    }
                });
            }
        });
    }
    
    

    public void EquipItemForUser(string userId, int itemId)
    {
        DatabaseReference userItemsRef = databaseReference.Child("Users").Child(userId).Child("equippedItems");
        userItemsRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving equipped items: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<int> items = new List<int>();
                if (snapshot.Exists && snapshot.ChildrenCount > 0)
                {
                    foreach (DataSnapshot childSnapshot in snapshot.Children)
                    {
                        int existingEquippedItemId = int.Parse(childSnapshot.Value.ToString());
                        items.Add(existingEquippedItemId);
                    }
                }
                items.Add(itemId);

                userItemsRef.SetValueAsync(items).ContinueWithOnMainThread(updateTask =>
                {
                    if (updateTask.IsFaulted)
                    {
                        Debug.LogError("Error updating equipped items: " + updateTask.Exception);
                    }
                    else if (updateTask.IsCompleted)
                    {
                        Debug.Log("Equipped item added successfully to user: " + userId);
                    }
                });
            }
        });
    }
    
    public void UnequipItemForUser(string userId, int itemId)
    {
        DatabaseReference userItemsRef = databaseReference.Child("Users").Child(userId).Child("equippedItems");
        userItemsRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving equipped items: " + task.Exception);
                return;
            }
        
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<int> items = new List<int>();
                if (snapshot.Exists && snapshot.ChildrenCount > 0)
                {
                    foreach (DataSnapshot childSnapshot in snapshot.Children)
                    {
                        int currentItemId = int.Parse(childSnapshot.Value.ToString());
                        if (currentItemId != itemId) 
                        {
                            items.Add(currentItemId);
                        }
                    }
                }

                userItemsRef.SetValueAsync(items).ContinueWithOnMainThread(updateTask =>
                {
                    if (updateTask.IsFaulted)
                    {
                        Debug.LogError("Error updating equipped items: " + updateTask.Exception);
                    }
                    else if (updateTask.IsCompleted)
                    {
                        Debug.Log("Equipped item removed successfully from user: " + userId);
                    }
                });
            }
        });
    }
    
    public void CheckIfChildIdExists(int childId, Action<bool> callback)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .OrderByChild("childID")
            .EqualTo(childId.ToString())
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || !task.IsCompleted)
                {
                    Debug.LogError("Error checking childId: " + task.Exception);
                    callback(false);
                }
                else
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists && snapshot.ChildrenCount > 0)
                    {
                        callback(true); 
                    }
                    else
                    {
                        callback(false);
                    }
                }
            });
    }


}

public static class ChildManager
{
    private static Dictionary<int, string> childNicknames = new Dictionary<int, string>();

    public static void AddChildNickname(int childId, string nickname)
    {
        if (!childNicknames.ContainsKey(childId))
        {
            childNicknames[childId] = nickname;
        }
    }

    public static string GetChildNickname(int childId)
    {
        if (childNicknames.TryGetValue(childId, out string nickname))
        {
            return nickname;
        }
        return string.Empty;
    }
}


[System.Serializable]
public class Child
{
    public int childID;
    public string nickname;

    public Child(int id, string name)
    {
        childID = id;
        nickname = name;
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
    public float todayCarbs;
    public float todayProtein;
    public float todayFat;
    public float todayCalories;

    public float requiredCarbs;
    public float requiredProtein;
    public float requiredFat;
    public float requiredCalories;
    public string nutritionalValuesUpdated; // time when nutritional values were updated

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
    public string childID;
    public List<int> boughtItems = new List<int>();
    public List<int> equipped = new List<int>();
    public Nutrient selectedFood;   // save selected food
    public User child;              // save selected child



    public User(string id, string username, string password, string email, string birthday, string registrationDate, 
                float _todayCarbs, float _todayProtein, float _todayFat, float _todayCalories, int points, int type, int age,
                int height, int weight, string gender, string goals, string newChildID)
    {
        Id = id;
        Username = username;
        Password = password; // Store passwords (not)securely
        Email = email;
        Birthday = birthday;
        SaveData = "";
        RegistrationDate = registrationDate;
        todayCarbs = _todayCarbs;
        todayProtein = _todayProtein;
        todayFat = _todayFat;
        todayCalories = _todayCalories;
        Points = points;
        userType = type;
        Age = age;
        Height = height;
        Weight = weight;
        Goals = goals;
        Gender = gender;
        childID = newChildID;

        nutritionalValuesUpdated = "";
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
    public string Id;
    public string Name;
    public string Date;
    public int Protein;
    public int Fat;
    public int Carbohydrates;
    public int Calories;
    public float Serving;
    public int Count;
    public string MealName;
    
    public Nutrient()
    {
    }

    public Nutrient(string id, string name, string date, int protein, int fat, int carbohydrates, int calories,
                    float serving, int count, string mealName)
    {
        Id = id;
        Name = name;
        Date = date;
        Protein = protein;
        Fat = fat;
        Carbohydrates = carbohydrates;
        Calories = calories;
        Serving = serving;
        Count = count;
        MealName = mealName;
    }
}

