using System;
using UnityEngine;
using TMPro; 
using UnityEngine.UI;

public class FormToPrefabSubmitter : MonoBehaviour
{
    public TMP_InputField[] inputFields;
    public GameObject prefabToInstantiate;
    public Transform prefabParent;
    
    public void OnSubmitButtonClick()
    {
        GameObject instantiatedPrefab = Instantiate(prefabToInstantiate, prefabParent ? prefabParent : null);
        User user = UserManager.Instance.CurrentUser;
        string taskId = Guid.NewGuid().ToString();
        user.Tasks.Add(new Task(taskId,inputFields[0].text, inputFields[1].text, "", "", "",10,false));
        if (user.userType != 0)
        {
            DatabaseManager.Instance.AddNewTask(user.Id, taskId, inputFields[0].text, inputFields[1].text, "", "", "",
                0,
                false);
        }
        TaskPrefabController controller = instantiatedPrefab.GetComponent<TaskPrefabController>();
        if (controller != null)
        {
            controller.Initialize(taskId, inputFields[0].text, inputFields[1].text, 10);
        }

        inputFields[0].text = "";
        inputFields[1].text = "";
    }
}
