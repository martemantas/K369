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

    public void SpawnUserTasks()
    {
        User user = UserManager.Instance?.CurrentUser;

        if (user != null && user.Tasks != null && user.Tasks.Count > 0)
        {
            List<Task> sortedTasks = new List<Task>();

            foreach (var task in user.Tasks)
            {
                DateTime expireDate;
                if (DateTime.TryParse(task.DateExpire, out expireDate))
                {
                    sortedTasks.Add(task);
                }
                else
                {
                    Debug.LogError("Failed to parse DateExpire for task: " + task.TaskId);
                    sortedTasks.Add(task);
                }
            }

            sortedTasks = sortedTasks.OrderBy(task => task.DateExpire).ToList();

            foreach (Task task in sortedTasks)
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
        else
        {
            Debug.Log("User has no tasks or user is null.");
        }
    }

}