using UnityEngine;
using TMPro; 

public class TogglePasswordVisibility : MonoBehaviour
{
    public TMP_InputField passwordInputField; 


    public void ShowPassword()
    {
        if (passwordInputField != null)
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Standard;
            passwordInputField.ForceLabelUpdate();
        }
    }

    public void HidePassword()
    {
        if (passwordInputField != null)
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
            passwordInputField.ForceLabelUpdate();
        }
    }
}
