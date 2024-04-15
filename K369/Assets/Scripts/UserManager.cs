using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance { get; private set; }
    public User CurrentUser { get; private set; }

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
        Debug.Log("tasks: " + user.Tasks.Count + ", meals: " + user.Meals.Count);
    }
}