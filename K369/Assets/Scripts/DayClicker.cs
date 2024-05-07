using System;
using UnityEngine;


public class DayClicker : MonoBehaviour
{
    [SerializeField] public GameObject taskLoaderObject;
    public GameObject mover; 
    [SerializeField] public int dayOffset = 0;
    private DateTime gameObjectDay;
    
    private void Start()
    {
        gameObjectDay = DateTime.Today.AddDays(dayOffset);
    }
    public void ClickAction()
    {
        if (mover != null)
        {
            mover.GetComponent<Mover>().MoveTo(transform.position.x);
        }
        if (taskLoaderObject != null)
        {
            TaskLoader loader = taskLoaderObject.GetComponent<TaskLoader>();
            if (loader != null)
            {
                Debug.Log("Clearing tasks and loading tasks for date: " + gameObjectDay);
                loader.ClearTasks();  
                loader.LoadTasksForDate(gameObjectDay);
            }
        }
    }
}