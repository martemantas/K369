using UnityEngine;
using UnityEngine.SceneManagement;

public class SwipeHandler : MonoBehaviour
{
    public float swipeThreshold = 50f;
    public string mainSceneName = "Main";
    public string timetableSceneName = "Timetable";
    public string nutritionSceneName = "Nutrition";

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    void Update()
    {
        DetectSwipe();
    }

    void DetectSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;

                float swipeMagnitude = (endTouchPosition - startTouchPosition).magnitude;
                if (swipeMagnitude > swipeThreshold)
                {
                    float swipeDirection = endTouchPosition.x - startTouchPosition.x;
                    if (SceneManager.GetActiveScene().name == mainSceneName)
                    {
                        if (swipeDirection < 0) // Left swipe
                        {
                            LoadScene(nutritionSceneName);
                        }
                        else if (swipeDirection > 0) // Right swipe
                        {
                            LoadScene(timetableSceneName);
                        }
                    }
                    else if (SceneManager.GetActiveScene().name == timetableSceneName && swipeDirection < 0)
                    {
                        LoadScene(mainSceneName);
                    }
                    else if (SceneManager.GetActiveScene().name == nutritionSceneName && swipeDirection > 0)
                    {
                        LoadScene(mainSceneName);
                    }
                }
            }
        }
    }

    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
