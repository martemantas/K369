using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonObjectManager : MonoBehaviour
{
    [System.Serializable]
    public class ButtonObjectPair
    {
        public Button button;
        public GameObject associatedObject;
    }

    public List<ButtonObjectPair> buttonObjectPairs; 
    private GameObject currentlyActiveObject; 

    private void Start()
    {
        foreach (var pair in buttonObjectPairs)
        {
            pair.button.onClick.AddListener(() => ButtonClicked(pair));
        }
    }

    private void ButtonClicked(ButtonObjectPair pair)
    {
        if (currentlyActiveObject != null)
        {
            currentlyActiveObject.SetActive(false); 
        }

        pair.associatedObject.SetActive(true); 
        currentlyActiveObject = pair.associatedObject; 
    }
}
