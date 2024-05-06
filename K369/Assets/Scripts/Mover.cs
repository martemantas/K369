using UnityEngine;

public class Mover : MonoBehaviour
{
    public float speed = 50.0f;  
    private float targetX;      
    private bool shouldMove = false;

    
    //Gyvenime taip nedaryti
    void Update()
    {
        if (shouldMove)
        {
            float step = speed * Time.deltaTime;  
            Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            if (Mathf.Abs(transform.position.x - targetX) < 0.001f)
            {
                shouldMove = false;
            }
        }
    }

    public void MoveTo(float x)
    {
        targetX = x;
        shouldMove = true;
    }
}