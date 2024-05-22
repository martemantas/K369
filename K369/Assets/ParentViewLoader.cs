using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentViewLoader : MonoBehaviour
{
    public GameObject parentView;
    public GameObject childView;
    public GameObject shopIcon;

    private void Start()
    {
        if (UserManager.Instance.CurrentUser.userType == 2 && UserManager.Instance.GetSelectedChildToViewID() != null)
        {
            parentView.SetActive(true);
            childView.SetActive(false);
            shopIcon.SetActive(false);
        }
        else
        {
            parentView.SetActive(false);
            childView.SetActive(true);
        }
    }
}
