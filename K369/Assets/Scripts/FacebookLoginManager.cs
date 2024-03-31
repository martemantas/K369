using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System;
using UnityEngine.SceneManagement;

public class FacebookLoginManager : MonoBehaviour
{
    public string MainScreenName = "Main screen";
    public string RegisterScreenName = "Register screen";

    void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    public void FacebookLogin()
    {
        var permissions = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(permissions, AuthCallback);
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            var aToken = AccessToken.CurrentAccessToken;
            FB.API("me?fields=id,email,name", HttpMethod.GET, FacebookAPICallback);
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    private void FacebookAPICallback(IGraphResult result)
    {
        if (result.Error != null)
        {
            Debug.LogError("Error getting Facebook user info: " + result.Error);
            return;
        }

        // Parse the user's Facebook data
        var id = result.ResultDictionary["id"].ToString();
        var email = result.ResultDictionary.ContainsKey("email") ? result.ResultDictionary["email"].ToString() : "";
        var name = result.ResultDictionary["name"].ToString();

        // Check if the user exists in your database
        StartCoroutine(CheckUserInDatabase(email, id, name));
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
    private IEnumerator CheckUserInDatabase(string email, string facebookId, string name)
    {
        bool emailNotRegistered = false;
        yield return StartCoroutine(IsEmailUnique(email, notExists => {
            emailNotRegistered = notExists;
        }));

        if (emailNotRegistered)
        {
            SceneManager.LoadScene(RegisterScreenName);

            yield break;
        }
        LoadScene();
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
    }}
