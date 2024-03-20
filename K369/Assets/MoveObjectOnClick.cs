using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Use this for UI elements

public class MoveObjectOnClick : MonoBehaviour
{
    public Button yourButton; 
    public GameObject objectToMove;
    public GameObject swipeManager;
    public float moveDistance = 2f; 
    public float moveSpeed = 1f; 

    private bool isMovingUp = false;
    private Vector3 originalPosition;
    private Vector3 newPosition;
    

    void Start()
    {
        originalPosition = objectToMove.transform.position;
        newPosition = originalPosition + new Vector3(0, moveDistance, 0);

        yourButton.onClick.AddListener(TogglePosition);
    }

    void TogglePosition()
    {
        StopAllCoroutines();
        swipeManager.SetActive(isMovingUp);
        StartCoroutine(MoveObject(isMovingUp ? originalPosition : newPosition));
        isMovingUp = !isMovingUp; 
    }

    IEnumerator MoveObject(Vector3 targetPosition)
    {
        while (Vector3.Distance(objectToMove.transform.position, targetPosition) > 0.01f)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null; 
        }
    }
}