using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Debug = fbg.Debug;

public class DropdownHandler : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public TaskPrefabController controller;
    public GameObject task;
    public TaskEditor editor;

    void Start()
    {
        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(OnDropdownChanged);
        }
    }
    

    private void OnDropdownChanged(int index)
    {
        switch (index)
        {
            case 1:
                editor.InitializeForm();
                break;
            case 2:
                DatabaseManager.Instance.DeleteTask(UserManager.Instance.CurrentUser.Id, controller.taskId);
                UserManager.Instance.DeleteTask(controller.taskId);
                Destroy(task);
                break;
        }

        dropdown.value = 0;
    }

    void OnDestroy()
    {
        if (dropdown != null)
        {
            dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        }
    }
}