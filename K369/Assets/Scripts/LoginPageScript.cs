using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginPageScript : MonoBehaviour
{
    public string StartScreenName = "Start screen";
    public string LoginScreenName = "Login screen";

    /// <summary>
    /// Opens start screen
    /// </summary>
    public void BackButtonAction()
    {
        SceneManager.LoadScene(StartScreenName);
    }

    /// <summary>
    /// Opens Login screen
    /// </summary>
    public void LoginButtonAction()
    {
        SceneManager.LoadScene(LoginScreenName);
    }
}
