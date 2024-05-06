using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowForParent : MonoBehaviour
{
    public GameObject gameObject;
    
    void Start()
    {
        if (UserManager.Instance.CurrentUser.userType != 2)
        {
            gameObject.SetActive(false);
        }
    }


}
