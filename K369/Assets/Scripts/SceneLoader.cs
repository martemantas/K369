using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public TMP_Text SceneToLoad;

    void Start()
    {
        SceneManager.LoadScene(SceneToLoad.text);
    }


}



