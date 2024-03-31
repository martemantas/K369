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

        User user = null; // Holds the retrieved user object
        yield return StartCoroutine(TryGetUserWithCredentials(userEmail, userPassword, fetchedUser => {
            user = fetchedUser;
        }));

        if (user == null)
        {
            errorMessage.text = errorIncorrectData;
            yield break;
        }

        // If execution reaches here, login is successful
        UserManager.Instance.LoginUser(user); // Save logged-in user
        LoadScene(); // Move to the next scene
    }

    private IEnumerator IsEmailUnique(string email, Action<bool> callback)
    {
        WaitForCallback wait = new WaitForCallback();
        bool isUnique = false;

        DatabaseManager.Instance.GetUserByEmail(email, (User user) =>
        {
            isUnique = user == null;
            wait.Complete(); // Indicate that the callback has been called
        });

        yield return wait;

        callback(isUnique);
    }

    private IEnumerator TryGetUserWithCredentials(string email, string password, Action<User> callback)
    {
        WaitForCallback wait = new WaitForCallback();
        User fetchedUser = null;

        DatabaseManager.Instance.GetUserByEmailAndPassword(email, password, (User user) =>
        {
            fetchedUser = user; // Directly use the user object returned from the database
            wait.Complete(); // Indicate that the callback has been called
        });

        yield return wait;

        callback(fetchedUser); // Pass the User object (null if credentials are incorrect)
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
