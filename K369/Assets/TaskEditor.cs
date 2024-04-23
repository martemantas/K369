using UnityEngine;
using TMPro;

public class TaskEditor : MonoBehaviour
{
    public TMP_InputField[] inputFields;
    private string currentTaskId;
    public GameObject form;
    public TaskPrefabController controller;
    private int pointsToAdd = 15;

    public void InitializeForm()
    {
        form.SetActive(true);
        currentTaskId = controller.taskId;
        Task task = UserManager.Instance.CurrentUser.Tasks.Find(t => t.TaskId == controller.taskId);
        if (task != null)
        {
            inputFields[0].text = task.Name;
            inputFields[1].text = task.Description;
        }
    }

    public void OnSubmitButtonClick()
    {
        User user = UserManager.Instance.CurrentUser;
        
        if (user.userType != 0 && currentTaskId != null) 
        {
            DatabaseManager.Instance.UpdateTaskDetails(user.Id, currentTaskId, inputFields[0].text, inputFields[1].text, "", "", "", 0, false);
            
        }
        Task taskToUpdate = user.Tasks.Find(t => t.TaskId == currentTaskId);
        
        if (taskToUpdate != null)
        {
            taskToUpdate.Name = inputFields[0].text;
            taskToUpdate.Description = inputFields[1].text;
        }
        controller.Initialize(controller.taskId,inputFields[0].text,inputFields[1].text, pointsToAdd);
        form.SetActive(false);
    }
}