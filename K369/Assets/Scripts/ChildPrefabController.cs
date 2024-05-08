using System.Collections;
using System.Collections.Generic;
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

    public void OnClick()
    {
        UserManager.Instance.SetSelectedChildToViewID(nameText.text);
        SceneManager.LoadScene(SceneToLoad);
    }

}



