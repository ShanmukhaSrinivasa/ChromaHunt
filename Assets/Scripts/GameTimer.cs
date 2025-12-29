using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float timeRemaining = 60f;
    public bool isGameActive = true;
    public TextMeshProUGUI timerText;

    private void Update()
    {
        if (isGameActive)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time is up!");
                timeRemaining = 0;
                isGameActive = false;
                //Trigger Game Over UI here later
            }
        }
    }

    private void DisplayTime(float timeToDisplay)
    {
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = "Time: " + seconds.ToString();
    }

    public void AddTime(float amount)
    {
        timeRemaining += amount;
    }

    public void SubtractTime(float amount)
    {
        timeRemaining -= amount;
    }
}
