using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR;

public class TaskPrefabController : MonoBehaviour
{
    public Text nameText;
    public Text descriptionText;
    public Button completeButton;
    public Image background;
    public string taskId;
    private int Points;

    [FormerlySerializedAs("_cookiePopupText")] [SerializeField] private GameObject _PopupText;

    public void Initialize(string id, string name, string description, int points)
    {
        taskId = id;
        nameText.text = name;
        descriptionText.text = description;
        Points = points;
    }

    public void MarkCompleted()
    {
        completeButton.GameObject().SetActive(false);
        background.color = new Color(0.6333f, 0.8392f, 0.5294f, 0.7098f);
    }

    public void OnCompleteButton()
    {
        User user = UserManager.Instance.CurrentUser;

        if (user.userType == 1) // child
        {
            Task task = user.Tasks.Find(t => t.TaskId == taskId);
            if (task != null)
            {
                task.Completed = true;
                DatabaseManager.Instance.MarkTaskAsCompleted(user.Id, taskId);
                if (!task.pointsGiven)
                {
                    task.pointsGiven = true;
                    UserManager.Instance.CurrentUser.Points += Points;
                    DatabaseManager.Instance.UpdateUserPoints(UserManager.Instance.CurrentUser.Id, UserManager.Instance.CurrentUser.Points);
                }
            }
            MarkCompleted();

            GameObject textPrefab = Instantiate(_PopupText, gameObject.transform);
            textPrefab.GetComponent<PopupText>().Setup(Points);
        }
    }
}

