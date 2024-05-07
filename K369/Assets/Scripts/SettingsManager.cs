using UnityEngine;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public TMP_InputField ageInputField;
    public TMP_Dropdown genderDropdown;
    public TMP_InputField heightInputField;
    public TMP_InputField weightInputField;

    private UserManager userManager;

    void Start()
    {
        userManager = UserManager.Instance;
        PopulateFields();
    }
    void PopulateFields()
    {
        string playerGender = userManager.GetPlayerGender();
        int index = GetGenderIndex(playerGender);

        ageInputField.text = userManager.GetPlayerAge().ToString();
        genderDropdown.value = index;
        heightInputField.text = userManager.GetPlayerHeight().ToString();
        weightInputField.text = userManager.GetPlayerWeight().ToString();
    }
    public void UpdateAge()
    {
        int newAge;
        if (int.TryParse(ageInputField.text, out newAge))
        {
            userManager.SetPlayerAge(newAge);
            DatabaseManager.Instance.UpdateUserAge(userManager.CurrentUser.Id, newAge);
        }
    }
    public void UpdateGender()
    {
        string newGender = genderDropdown.options[genderDropdown.value].text;
        userManager.SetPlayerGender(newGender);
        DatabaseManager.Instance.UpdateUserGender(userManager.CurrentUser.Id, newGender);
    }

    public void UpdateHeight()
    {
        int newHeight;
        if (int.TryParse(heightInputField.text, out newHeight))
        {
            userManager.SetPlayerHeight(newHeight);
            DatabaseManager.Instance.UpdateUserHeight(userManager.CurrentUser.Id, newHeight);
        }
    }

    public void UpdateWeight()
    {
        int newWeight;
        if (int.TryParse(weightInputField.text, out newWeight))
        {
            userManager.SetPlayerWeight(newWeight);
            DatabaseManager.Instance.UpdateUserWeight(userManager.CurrentUser.Id, newWeight);
        }
    }
    private int GetGenderIndex(string gender)
    {
        if (gender == null)
            return 0;
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
