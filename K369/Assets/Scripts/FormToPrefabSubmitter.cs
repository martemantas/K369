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
        user.Tasks.Add(new Task(inputFields[0].text, inputFields[1].text, "", "", "",0,false));
        if (user.userType != 0)
        {
            string taskId = Guid.NewGuid().ToString();
            DatabaseManager.Instance.AddNewTask(user.Id, taskId, inputFields[0].text, inputFields[1].text, "", "", "",
                0,
                false);
        }

        Text[] textComponents = instantiatedPrefab.GetComponentsInChildren<Text>();
        
        for (int i = 0; i < inputFields.Length && i < textComponents.Length; i++)
        {
            textComponents[i].text = inputFields[i].text;
        }
    }
}
