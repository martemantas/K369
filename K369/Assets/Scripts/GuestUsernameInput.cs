using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Windows;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GuestUsernameInput : MonoBehaviour
{
    public TextMeshProUGUI output;      // Display greeting to user with his username
    public TMP_InputField username;     // input from user

    // Validation settings
    int usernameLengthMin = 3;
    int usernameLengthMax = 7;

    // Validation error messages
    public string usernameLengthError = "Username length is incorrect.";
    public string usernameExistanceError = "Username is taken.";
    public string usernameFormatError = "Username format is incorrect.";
    public string successMessage = "Welcome, ";
    public bool canEnter = false;       // Check if user can enter the Main screen

    // Screens load settings
    public string MainMenuScreenName = "Main screen";
    public string GuestScreenName = "Guest screen";

    /// <summary>
    /// Validation process when button 'Tikrinti' is pressed
    /// </summary>
    public void ValidateButtonAction()
    {
        output.text = "";

        if (username != null && !string.IsNullOrEmpty(username.text))
        {
            output.text = ValidateUsername(username.text, out canEnter);
        }
        else
        {
            output.text = usernameLengthError;
        }

    }

    /// <summary>
    /// Main screen opening process when button 'Pradëti' is pressed
    /// </summary>
    public void LoadButtonAction()
    {
        ValidateButtonAction();
        if (canEnter)
        {
            SceneManager.LoadScene(MainMenuScreenName);
        }
        else
        {
            output.text = usernameLengthError;
        }
    }

    /// <summary>
    /// Guest screen opening process when button 'Tæsti kaip sveèias' is pressed
    /// </summary>
    public void EnterAsGuestButtonAction()
    {
        SceneManager.LoadScene(GuestScreenName);     
    }

    /// <summary>
    /// Validates username. If correct, user can access main screen
    /// </summary>
    /// <param name="username">Username (input)</param>
    /// <param name="canEnter">Flag to check if user can enter</param>
    /// <returns>validation message</returns>
    public string ValidateUsername(string username, out bool canEnter)
    {
        string pattern = @"^[a-zA-Z0-9\s\p{P}]+$";
        canEnter = false;
        
        if (username.Length < usernameLengthMin || 
            username.Length > usernameLengthMax) 
        {
            return usernameLengthError;
        }

        if (!Regex.IsMatch(username, pattern))
        {
            return usernameFormatError;
        }

        // ! Need to check the database !
        bool exist = false;
        if (exist)
        {
            return usernameExistanceError;
        }

        else
        {
            canEnter = true;
            return successMessage + username;
        }
    }

}
