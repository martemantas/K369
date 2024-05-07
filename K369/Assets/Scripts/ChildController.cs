using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChildController : MonoBehaviour
{
    public TMP_InputField childID;

    public void OnAddButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload settings screen
    }

}



