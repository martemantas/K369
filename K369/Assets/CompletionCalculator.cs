using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompletionCalculator : MonoBehaviour
{
    private User currentUser;

    public float mealCompletionPercentage;
    public float taskCompletionPercentage;
    public float receivedPointsPercentage;
    public int completedMealsCount;
    public int completedTasksCount;
    public int totalReceivedPoints;
    public int totalPossiblePoints;
    public float score;

    public Image mealCompletionImage;
    public Image taskCompletionImage;
    public Image pointsCompletionImage;
    public TMP_Text mealCompletionText;
    public TMP_Text taskCompletionText;
    public TMP_Text pointsCompletionText;
    public TMP_Text scoreText;

    private async void Start()
    {
        currentUser = UserManager.Instance.CurrentUser;
        await CalculateChildCompletionPercentages(currentUser);
        UpdateUI();
    }

    public async System.Threading.Tasks.Task CalculateChildCompletionPercentages(User user)
    {
        if (user == null)
        {
            Debug.LogWarning("User is not assigned.");
            return;
        }

        User child = await FindUserChild(user);

        if (child == null)
        {
            Debug.LogWarning("No child assigned to the current user.");
            return;
        }

        await LoadUserMealsAndTasks(child);

        DateTime startOfWeek = GetStartOfWeek(DateTime.Now);
        DateTime endOfWeek = startOfWeek.AddDays(7);

        List<Meal> childMealsThisWeek = GetMealsThisWeek(child.Meals, startOfWeek, endOfWeek);
        List<Task> childTasksThisWeek = GetTasksThisWeek(child.Tasks, startOfWeek, endOfWeek);

        mealCompletionPercentage = CalculateCompletionPercentage(childMealsThisWeek, out completedMealsCount, out int mealPoints, out int possibleMealPoints);
        taskCompletionPercentage = CalculateCompletionPercentage(childTasksThisWeek, out completedTasksCount, out int taskPoints, out int possibleTaskPoints);

        totalReceivedPoints = mealPoints + taskPoints;
        totalPossiblePoints = possibleMealPoints + possibleTaskPoints;

        receivedPointsPercentage = totalPossiblePoints > 0 ? (float)totalReceivedPoints / totalPossiblePoints : 0;

        CalculateScore(childMealsThisWeek, childTasksThisWeek);
    }

    public async Task<User> FindUserChild(User user)
    {
        if (user.userType == 2) // parent
        {
            string childID = UserManager.Instance.GetSelectedChildToViewID();
            User userChild = await DatabaseManager.Instance.FindUserByChildID(childID);
            return userChild;
        }
        return user;
    }

    private async System.Threading.Tasks.Task LoadUserMealsAndTasks(User user)
    {
        var mealsTask = new TaskCompletionSource<List<Meal>>();
        var tasksTask = new TaskCompletionSource<List<Task>>();

        DatabaseManager.Instance.GetUserMeals(user.Id, (List<Meal> mealList) =>
        {
            user.Meals = mealList;
            mealsTask.SetResult(mealList);
        });

        DatabaseManager.Instance.GetUserTasks(user.Id, (List<Task> taskList) =>
        {
            user.Tasks = taskList;
            tasksTask.SetResult(taskList);
        });

        await System.Threading.Tasks.Task.WhenAll(mealsTask.Task, tasksTask.Task);
    }

    private void UpdateUI()
    {
        mealCompletionImage.fillAmount = mealCompletionPercentage;
        taskCompletionImage.fillAmount = taskCompletionPercentage;
        pointsCompletionImage.fillAmount = receivedPointsPercentage;

        mealCompletionText.text = completedMealsCount.ToString();
        taskCompletionText.text = completedTasksCount.ToString();
        pointsCompletionText.text = totalReceivedPoints.ToString();
        scoreText.text = score.ToString("0.00");
    }

    private DateTime GetStartOfWeek(DateTime date)
    {
        int diff = date.DayOfWeek - DayOfWeek.Monday;
        if (diff < 0)
        {
            diff += 7;
        }
        return date.AddDays(-1 * diff).Date;
    }

    private List<Meal> GetMealsThisWeek(List<Meal> meals, DateTime startOfWeek, DateTime endOfWeek)
    {
        List<Meal> mealsThisWeek = new List<Meal>();

        foreach (var meal in meals)
        {
            if (DateTime.Parse(meal.DateExpire) >= startOfWeek && DateTime.Parse(meal.DateExpire) <= endOfWeek)
            {
                mealsThisWeek.Add(meal);
            }
        }

        return mealsThisWeek;
    }

    private List<Task> GetTasksThisWeek(List<Task> tasks, DateTime startOfWeek, DateTime endOfWeek)
    {
        List<Task> tasksThisWeek = new List<Task>();

        foreach (var task in tasks)
        {
            if (DateTime.Parse(task.DateExpire) >= startOfWeek && DateTime.Parse(task.DateExpire) <= endOfWeek)
            {
                tasksThisWeek.Add(task);
            }
        }

        return tasksThisWeek;
    }

    private float CalculateCompletionPercentage<T>(List<T> items, out int completedCount, out int totalPoints, out int possiblePoints) where T : class
    {
        completedCount = 0;
        totalPoints = 0;
        possiblePoints = 0;

        foreach (var item in items)
        {
            if (item is Meal meal)
            {
                possiblePoints += meal.Points;
                if (meal.Completed)
                {
                    completedCount++;
                    totalPoints += meal.Points;
                }
            }
            else if (item is Task task)
            {
                possiblePoints += task.Points;
                if (task.Completed)
                {
                    completedCount++;
                    totalPoints += task.Points;
                }
            }
        }

        return items.Count == 0 ? 0 : (float)completedCount / items.Count;
    }

    private void CalculateScore(List<Meal> meals, List<Task> tasks)
    {
        score = 8f;

        foreach (var meal in meals)
        {
            if (meal.Completed && DateTime.Parse(meal.DateExpire) >= DateTime.Now)
            {
                score = Mathf.Clamp(score + 0.25f, 0f, 10f);
            }
            else if (!meal.Completed && DateTime.Parse(meal.DateExpire) < DateTime.Now)
            {
                score = Mathf.Clamp(score - 0.25f, 0f, 10f);
            }
        }

        foreach (var task in tasks)
        {
            if (task.Completed && DateTime.Parse(task.DateExpire) >= DateTime.Now)
            {
                score = Mathf.Clamp(score + 0.25f, 0f, 10f);
            }
            else if (!task.Completed && DateTime.Parse(task.DateExpire) < DateTime.Now)
            {
                score = Mathf.Clamp(score - 0.25f, 0f, 10f);
            }
        }
    }
}
