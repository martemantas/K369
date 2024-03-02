using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Logout : MonoBehaviour
{
    public string StartScreenName = "Start screen";
    //Takes user to main screen.
    //Function should be expanded later
    public void LogoutAction()
    {
        SceneManager.LoadScene(StartScreenName);
    }
    
}
