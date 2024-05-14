using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstantiatePrefab : MonoBehaviour
{
    public GameObject prefabToInstantiate;
    public Transform prefabParent;
    public TMP_InputField IdInput;
    public TMP_Text errorMessageText;
    public GameObject modal;
    public ChildController childController;

    public void InstantiatePrefabMethod()
    {
        int enteredChildId = int.Parse(IdInput.text);

        if (UserManager.Instance.CurrentUser.children.Contains(enteredChildId))
        {
            errorMessageText.text = "Incorrect ID";
            return;
        }

        DatabaseManager.Instance.CheckIfChildIdExists(enteredChildId, exists =>
        {
            if (exists)
            {
                DatabaseManager.Instance.AddChildToUser(UserManager.Instance.CurrentUser.Id, enteredChildId);
                UserManager.Instance.CurrentUser.children.Add(enteredChildId);
                GameObject instantiatedPrefab = Instantiate(prefabToInstantiate, prefabParent ? prefabParent : null);
                IdInput.text = "";
                errorMessageText.text = "";
                modal.SetActive(false);
                childController.OnAddButton();
            }
            else
            {
                errorMessageText.text = "Incorrect ID";
            }
        });
    }
}