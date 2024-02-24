using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegisterPageScript : MonoBehaviour
{
    public string StartScreenName = "Start screen";
    public string RegisterScreenName = "Register screen";

    /// <summary>
    /// Opens start screen
    /// </summary>
    public void BackButtonAction()
    {
        SceneManager.LoadScene(StartScreenName);
    }

    /// <summary>
    /// Opens Register screen
    /// </summary>
    public void RegisterButtonAction()
    {
        SceneManager.LoadScene(RegisterScreenName);
    }
}
