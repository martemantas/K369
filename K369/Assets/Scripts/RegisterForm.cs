using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterForm : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;
    public TMP_Dropdown dayDropdown;
    public TMP_Dropdown monthDropdown;
    public TMP_Dropdown yearDropdown;
    public DatabaseManager databaseManager;
    public TMP_Text errorMessage;

    private void Start()
    {
        PopulateDropdowns();
    }

    public void OnRegisterButtonClicked()
    {
        StartCoroutine(TryRegister());
    }
    private IEnumerator TryRegister()
    {
        errorMessage.text = "";

        if (passwordField.text != confirmPasswordField.text)
        {
            errorMessage.text = "Passwords must match";
            yield break; // Exit the Coroutine
        }

        if (!IsValidEmail(emailField.text))
        {
            errorMessage.text = "Must be a valid email address";
            yield break; // Exit the Coroutine
        }

        bool emailIsUnique = false; 
        yield return StartCoroutine(IsEmailUnique(emailField.text, isUnique => {
            emailIsUnique = isUnique;
        }));

        if (!emailIsUnique)
        {
            errorMessage.text = "User with this email already exists";
            yield break; 
        }

        string userId = Guid.NewGuid().ToString();
        string registrationDate = DateTime.Now.ToString("yyyy-MM-dd");
        string dob = GetFormattedDateOfBirth();
        databaseManager.AddNewUser(userId, usernameField.text, passwordField.text, emailField.text, dob, registrationDate);
    }

    
    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
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

    
    void PopulateDropdowns()
    {
        // Days
        dayDropdown.ClearOptions();
        List<string> days = new List<string>();
        for (int day = 1; day <= 31; day++)
        {
            days.Add(day.ToString());
        }
        dayDropdown.AddOptions(days);

        // Months
        monthDropdown.ClearOptions();
        List<string> months = new List<string>{"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};
        monthDropdown.AddOptions(months);

        // Years
        yearDropdown.ClearOptions();
        List<string> years = new List<string>();
        for (int year = System.DateTime.Now.Year; year >= 1900; year--)
        {
            years.Add(year.ToString());
        }
        yearDropdown.AddOptions(years);
    }
    
    private string GetFormattedDateOfBirth()
    {
        string day = dayDropdown.options[dayDropdown.value].text;
        string month = (monthDropdown.value + 1).ToString().PadLeft(2, '0');
        string year = yearDropdown.options[yearDropdown.value].text;
        return $"{year}-{month}-{day}";
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
