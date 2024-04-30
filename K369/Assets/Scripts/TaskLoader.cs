using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TaskLoader : MonoBehaviour
{
    public GameObject prefabToInstantiate;
    public Transform prefabParent;

    private void Start()
    {
        SpawnUserTasks();
    }

    public void ClearTasks()
    {
        foreach (Transform child in prefabParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void LoadTasksForDate(DateTime date)
    {
        User user = UserManager.Instance?.CurrentUser;
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

    public void SpawnUserTasks()
    {
        DateTime today = DateTime.Today;
        var todayFormatted = today.ToString("yyyy-MM-dd");

        User user = UserManager.Instance?.CurrentUser;
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
    }

    private void CreateTaskInstances(List<Task> tasks)
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
    }
}
