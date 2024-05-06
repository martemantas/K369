using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildLoader : MonoBehaviour
{
    public GameObject prefabToInstantiate;
    public Transform prefabParent;

    private void Start()
    {
        if (UserManager.Instance == null)
            return;
        InstantiateChildren();
    }
    
    public void InstantiateChildren()
    {
        UserManager.Instance.CurrentUser.children.ForEach(child =>
        {
            GameObject instantiatedPrefab = Instantiate(prefabToInstantiate, prefabParent ? prefabParent : null);
        });
    }
}
