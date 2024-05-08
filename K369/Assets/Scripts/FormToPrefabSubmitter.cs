using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class FormToPrefabSubmitter : MonoBehaviour
{
    public TMP_InputField[] inputFields;
    public GameObject prefabToInstantiate;
    public Transform prefabParent;
    public TMP_Text errorLabel;
    public DateTimePicker dateTimePicker;
    private int pointsToAdd = 15;
    public GameObject timetableScreen;

    public void OnSubmitButtonClick()
    {
        errorLabel.text= "";
        if(inputFields[0].text == "")
        {
            errorLabel.text = "Task name cannot be empty";
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload screen

        DateTime selectedDateTime = dateTimePicker.GetSelectedDateTime();
        string formattedDate = selectedDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        GameObject instantiatedPrefab = Instantiate(prefabToInstantiate, prefabParent ? prefabParent : null);
        User user = UserManager.Instance.CurrentUser;
        string taskId = Guid.NewGuid().ToString();
        Task task = new Task(taskId, inputFields[0].text, inputFields[1].text, DateTime.Now.ToString(), "", formattedDate, pointsToAdd, false);
        user.Tasks.Add(task);

        if (user.userType == 1)
        {
            DatabaseManager.Instance.AddNewTask(user.Id, taskId, inputFields[0].text, inputFields[1].text, DateTime.Now.ToString(), "", formattedDate,
                pointsToAdd,
                false);
        }
        else if (user.userType == 2)
        {
            string childID = UserManager.Instance.GetSelectedChildToViewID();
            DatabaseManager.Instance.AddNewTaskForChild(childID, taskId, inputFields[0].text, inputFields[1].text, DateTime.Now.ToString(), "", formattedDate,
                pointsToAdd,
                false);
        }

        if (dateTimePicker.GetSelectedDateTime().ToString().StartsWith(DateTime.Today.ToString()))
        {
            TaskPrefabController controller = instantiatedPrefab.GetComponent<TaskPrefabController>();
            if (controller != null)
            {
                controller.Initialize(taskId, inputFields[0].text, inputFields[1].text, pointsToAdd);
            }
        }

        inputFields[0].text = "";
        inputFields[1].text = "";
        timetableScreen.SetActive(true);
    }

}