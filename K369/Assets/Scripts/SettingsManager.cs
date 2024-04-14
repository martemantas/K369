using UnityEngine;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public TMP_InputField ageInputField;
    public TMP_Dropdown genderDropdown;

    void Start()
    {
        UserManager userManager = UserManager.Instance;

        int playerAge = userManager.GetPlayerAge();
        string playerGender = userManager.GetPlayerGender();

        int index = GetGenderIndex(playerGender);
        ageInputField.text = playerAge.ToString();
        genderDropdown.value = index;
    }
    private int GetGenderIndex(string gender)
    {
        switch (gender.ToLower())
        {
            case "male":
                return 0;
            case "female":
                return 1;
            case "other":
                return 2;
            default:
                return 0;
        }
    }
}
