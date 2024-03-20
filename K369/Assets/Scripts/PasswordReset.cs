using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PasswordReset : MonoBehaviour
{
    public TMP_InputField emailField;
    public TMP_Text errorMessage;
    public GameObject enterScreen;
    public GameObject confirmScreen;

    
    public void TryReset()
    {
        errorMessage.text = "";

        if (!IsValidEmail(emailField.text))
        {
            errorMessage.text = "Must be a valid email address";
        }
        else
        {
            enterScreen.SetActive(false);
            confirmScreen.SetActive(true);
        }

    }
    
    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}
