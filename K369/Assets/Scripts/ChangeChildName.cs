using TMPro;
using UnityEngine;

public class ChangeChildName : MonoBehaviour
{
    public TMP_Text childIDField;
    public TMP_InputField childNameField;

    public void ChangeName()
    {
        string childId = childIDField.text;
        string newName = childNameField.text;

        DatabaseManager.Instance.UpdateChildName(childId, newName, (bool success) =>
        {
            if (success)
            {
                Debug.Log("Child name updated successfully.");
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
