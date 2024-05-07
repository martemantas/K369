using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TaskLoader : MonoBehaviour
{
    public GameObject prefabToInstantiate;
    public Transform prefabParent;

    private string childID;
    private DatabaseManager databaseManager;

    private void Start()
    {
        databaseManager = new DatabaseManager();
        Spawn();
    }

    public async void Spawn()
    {
        User currentUser = UserManager.Instance.CurrentUser;
        if (currentUser.userType == 2) // parent
        {
            childID = UserManager.Instance.GetSelectedChildToViewID();
            User userChild = await FindUserChild();
            Debug.Log("Task loader: spawning tasks for user (childID): " + userChild.childID);
            SpawnUserTasks(userChild);
        }
        else
        {
            SpawnUserTasks(currentUser);
        }

    }

    public void ClearTasks()
    {
        foreach (Transform child in prefabParent)
        {
            Destroy(child.gameObject);
        }
    }

    public async void LoadTasksForDate(DateTime date)
    {
        User user;
        User currentUser = UserManager.Instance?.CurrentUser;
        if (currentUser.userType == 2) // parent
        {
            childID = UserManager.Instance.GetSelectedChildToViewID();
            User userChild = await FindUserChild();
            Debug.Log("Task loader: spawning tasks for date for user (childID): " + userChild.childID);

            user = userChild;
        }
        else
        {
            user = currentUser;
        }
        if (user != null && user.Tasks != null && user.Tasks.Count > 0)
        {
            var dateFormatted = date.ToString("yyyy-MM-dd");
            List<Task> sortedTasks = user.Tasks
                .Where(task => task.DateExpire.StartsWith(dateFormatted))
                .OrderBy(task => task.DateExpire)
                .ToList();

            CreateTaskInstances(sortedTasks);
        }
        else
        {
            Debug.Log("User has no tasks or user is null.");
        }
    }

    public void SpawnUserTasks(User user)
    {
        DateTime today = DateTime.Today;
        var todayFormatted = today.ToString("yyyy-MM-dd");
        databaseManager.GetUserTasks(user.Id, (List<Task> tasks) =>
        {
            if (user != null && user.Tasks != null && user.Tasks.Count > 0)
            {
                var sortedTasks = user.Tasks
                    .Where(task => task.DateExpire.StartsWith(todayFormatted))
                    .OrderBy(task => task.DateExpire)
                    .ToList();

                CreateTaskInstances(sortedTasks);
            }
            else
            {
                Debug.Log("User has no tasks or user is null.");
            }
        });
    }

    

    private async void CreateTaskInstances(List<Task> tasks)
    {
        User user;
        User currentUser = UserManager.Instance?.CurrentUser;
        if (currentUser.userType == 2) // parent
        {
            childID = UserManager.Instance.GetSelectedChildToViewID();
            User userChild = await FindUserChild();
            Debug.Log("Task loader: spawning tasks for date for user (childID): " + userChild.childID);
            user = userChild;
        }
        else
        {
            user = currentUser;
        }
        databaseManager.GetUserTasks(user.Id, (List<Task> tasks) =>
        {
            foreach (Task task in tasks)
            {
                GameObject taskInstance = Instantiate(prefabToInstantiate, prefabParent);
                TaskPrefabController controller = taskInstance.GetComponent<TaskPrefabController>();
                if (controller != null)
                {
                    controller.Initialize(task.TaskId, task.Name, task.Description, task.Points);
                    if (task.Completed)
                    {
                        controller.MarkCompleted();
                    }
                }
            }
        });
    }

    public async Task<User> FindUserChild()
    {
        User user = await databaseManager.FindUserByChildID(childID);
        return user;

    }

}
