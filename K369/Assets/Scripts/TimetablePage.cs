using UnityEngine;
using UnityEngine.UI;

public class CreatePrefabOnClick : MonoBehaviour
{
    public GameObject prefabToInstantiate; // Reference to the prefab to instantiate
    public Transform contentPanel; // Reference to the content panel where the prefab will be instantiated

    void Start()
    {
        
    }

    public void AddNewTask()
    {
        // Instantiate the prefab in the content panel
        GameObject instantiatedPrefab = Instantiate(prefabToInstantiate, contentPanel);
    }
}
