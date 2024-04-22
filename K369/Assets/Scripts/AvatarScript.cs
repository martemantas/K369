using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarScript : MonoBehaviour
{
    public Vector2 touchPosition;
    private Touch touch;
    private Quaternion rotationY;
    private float rotateSpeedModifier = 0.1f;


    void Update()
    {
        RotateBySwipeControls();             
    }

    // Rotates avatar by swiping
    private void RotateBySwipeControls()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                rotationY = Quaternion.Euler(
                    0f,
                    -touch.deltaPosition.x * rotateSpeedModifier,
                    0f);

                transform.rotation = rotationY * transform.rotation;
            }
        }
    }

}
