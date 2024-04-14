using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance { get; private set; }
    public User CurrentUser { get; private set; }
    private int playerAge;
    private string playerGender;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoginUser(User user)
    {
        CurrentUser = user;
        SetPlayerAge(user.Age);
        SetPlayerGender(user.Gender);
    }
    public void SetPlayerAge(int age)
    {
        playerAge = age;
    }

    public int GetPlayerAge()
    {
        return playerAge;
    }
    public void SetPlayerGender(string gender)
    {
        playerGender = gender;
    }

    public string GetPlayerGender()
    {
        return playerGender;
    }
}