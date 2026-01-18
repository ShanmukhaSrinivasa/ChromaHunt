using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float timeRemaining = 60f;
    public bool isGameActive = true;
    public TextMeshProUGUI timerText;
    public int score = 0;
    public TextMeshProUGUI scoreText;

    private void Update()
    {
        if (isGameActive)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.unscaledDeltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                EndGame();
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
        if (!isGameActive || timeRemaining <= 0)
        {
            return;
        }
        timeRemaining += amount;
    }

    public void SubtractTime(float amount)
    {
        timeRemaining -= amount;
    }

    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "Bounties: " + score;
    }

    private void EndGame()
    {
        Debug.Log("Time is up!");
        timeRemaining = 0;
        isGameActive = false;
        DisplayTime(0);
    }
}
