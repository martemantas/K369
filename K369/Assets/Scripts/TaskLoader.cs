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
        User user = UserManager.Instance.CurrentUser;

        if (user != null && user.Tasks != null && user.Tasks.Count > 0)
        {
            foreach (Task task in user.Tasks)
            {
                GameObject taskInstance = Instantiate(prefabToInstantiate, prefabParent);
                TaskPrefabController controller = taskInstance.GetComponent<TaskPrefabController>();
                if (controller != null)
                {
                    controller.Initialize(task.TaskId, task.Name, task.Description, task.Points);
                    if(task.Completed)
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