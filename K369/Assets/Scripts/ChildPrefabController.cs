using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public async void DeleteChild()
    {
        User parent = UserManager.Instance.CurrentUser;
        string childId = nameText.text;
        GameObject parentObject = transform.parent.gameObject;

        DatabaseManager.Instance.RemoveChildFromParent(parent.Id, childId, (bool isRemoved) => {
            if (isRemoved)
            {
                // Remove the prefab from the scene
                Destroy(parentObject);
                Debug.Log("child removed from children list");
            }
            else
            {
                Debug.LogError("Failed to remove child from parent's list in database.");
            }
        });
    }
}



