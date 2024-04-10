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
            Debug.Log(user.Tasks.Count);
            foreach (Task task in user.Tasks)
            {
                Debug.Log(task.Description);
                GameObject taskInstance = Instantiate(prefabToInstantiate, prefabParent);

                Text[] textComponents = taskInstance.GetComponentsInChildren<Text>();
                if (textComponents.Length > 0)
                {
                    textComponents[0].text = task.Name;
                    if (textComponents.Length > 1)
                    {
                        textComponents[1].text = task.Description;
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
