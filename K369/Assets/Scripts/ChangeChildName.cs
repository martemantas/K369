using TMPro;
using UnityEngine;

public class ChangeChildName : MonoBehaviour
{
    public TMP_Text childIDField;
    public TMP_InputField childNameField;
    public TMP_Text usernameText;

    public void ChangeName()
    {
        string childId = childIDField.text;
        string newName = childNameField.text;

        DatabaseManager.Instance.UpdateChildName(childId, newName, (bool success) =>
        {
            if (success)
            {
                Debug.Log("Child name updated successfully.");
                // Update the username text TMP value if not empty
                if (!string.IsNullOrEmpty(newName))
                {
                    usernameText.text = newName;
                }
                // Add any additional logic here after successfully updating the child's name
            }
            else
            {
                Debug.LogError("Failed to update child name.");
                // Add any error handling logic here
            }
        });
    }
}
