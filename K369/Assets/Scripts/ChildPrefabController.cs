using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChildPrefabController : MonoBehaviour
{
    public TMP_Text nameText;
    private string SceneToLoad = "Main screen";

    public void Initialize(string name)
    {
        nameText.text = name;
    }

    // On child selected, save selected child to current user
    public async void OnClick()
    {
        User parent = UserManager.Instance.CurrentUser;
        User child = await FindUserChild();
        parent.child = child;
        UserManager.Instance.SetSelectedChildToViewID(nameText.text);
        SceneManager.LoadScene(SceneToLoad);
    }

    private async Task<User> FindUserChild()
    {
        DatabaseManager databaseManager = new DatabaseManager();
        User user = await databaseManager.FindUserByChildID(nameText.text);
        return user;

    }

}



