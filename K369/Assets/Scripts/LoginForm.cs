using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class LoginForm : MonoBehaviour
{
    public TMP_InputField email;
    public TMP_InputField password;
    public TMP_Text errorMessage;
    public DatabaseManager databaseManager;
    public string MainScreenName = "Main screen";

    // Error messages
    public string errorEmptyEmail         = "Enter your email address";
    public string errorNotValidEmail      = "Not valid email address";
    public string errorEmptyPassword      = "Enter your password";
    public string errorIncorrectData      = "Incorrect email or password";
    public string errorEmailNotRegistered = "Email not registered";

    /// <summary>
    /// Tries to login user and redirect to Main screen
    /// </summary>
    public void OnLoginButtonPressed()
    {
        StartCoroutine(TryLogin());
    }

    public IEnumerator TryLogin()
    {
        string userEmail = email.text;
        string userPassword = password.text;
        errorMessage.text = "";

        if (userEmail.Length == 0 && userPassword.Length == 0
            || userEmail.Length == 0)
        {
            errorMessage.text = errorEmptyEmail;
            yield break;
        }

        if (!IsValidEmail(userEmail))
        {
            errorMessage.text = errorNotValidEmail;
            yield break;
        }

        if (userPassword.Length == 0)
        {
            errorMessage.text = errorEmptyPassword;
            yield break;
        }

        bool emailNotRegistered = false; // Email in database check
        yield return StartCoroutine(IsEmailUnique(userEmail, notExists => {
            emailNotRegistered = notExists;
        }));

        if (emailNotRegistered)
        {
            errorMessage.text = errorEmailNotRegistered;
            yield break;
        }

        bool credentialsNotRegistered = false; // Credentials in database check
        yield return StartCoroutine(AreCredentialsNotUsed(userEmail, userPassword, notExists => {
            credentialsNotRegistered = notExists;
        }));

        if (credentialsNotRegistered)
        {
            errorMessage.text = errorIncorrectData;
            yield break;
        }

        // User input data is correct (registered in database)
        if (!credentialsNotRegistered)
        {
            LoadScene();
        }
    }

    private IEnumerator IsEmailUnique(string email, Action<bool> callback)
    {
        WaitForCallback wait = new WaitForCallback();
        bool isUnique = false;

        databaseManager.GetUserByEmail(email, (User user) =>
        {
            isUnique = user == null;
            wait.Complete(); // Indicate that the callback has been called
        });

        yield return wait;

        callback(isUnique);
    }

    private IEnumerator AreCredentialsNotUsed(string email, string password, Action<bool> callback)
    {
        WaitForCallback wait = new WaitForCallback();
        bool notExists = false;

        databaseManager.GetUserByEmailAndPassword(email, password, (User user) =>
        {
            notExists = user == null;
            wait.Complete(); // Indicate that the callback has been called
        });

        yield return wait;

        callback(notExists);
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(MainScreenName);
    }

    public class WaitForCallback : CustomYieldInstruction
    {
        private bool isDone = false;

        public override bool keepWaiting => !isDone;

        public void Complete()
        {
            isDone = true;
        }
    }
}
