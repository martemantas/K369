using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstantiatePrefab : MonoBehaviour
{
    public GameObject prefabToInstantiate;
    public Transform prefabParent;
    public TMP_InputField IdInput;
    public void InstantiatePrefabMethod()
    {
        DatabaseManager.Instance.AddChildToUser(UserManager.Instance.CurrentUser.Id, int.Parse(IdInput.text));
        if (UserManager.Instance == null)
            return;
        UserManager.Instance.CurrentUser.children.Add(int.Parse(IdInput.text));
        GameObject instantiatedPrefab = Instantiate(prefabToInstantiate, prefabParent ? prefabParent : null);
        IdInput.text = "";
    }
}
