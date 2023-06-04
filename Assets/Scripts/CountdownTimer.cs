using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public float totalTime = 60f; // total time in seconds

    private float currentTime;
    private bool timerStarted = false; 
    public bool timerEnded {get; private set;} = false;

    public TMP_Text timerText;

    private void Start()
    {
        StartTimer();
        currentTime = totalTime;
        UpdateTimerText();
    }

    private void Update()
    {
        if (timerEnded)
        {
            setTimerToTime(0, 0);
            return;
        }

        currentTime -= Time.deltaTime;

        UpdateTimerText();

        if (currentTime <= 0f /*&& !timerEnded*/)
        {
            timerEnded = true;
            GameObject.FindGameObjectWithTag("Painting").GetComponent<PaintingController>().submitWall();
            GameObject.FindGameObjectWithTag("Game Controller").GetComponent<GameController>().endGame();
        }
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartTimer()
    {
        timerStarted = true;
    }

    public void ResetTimer()
    {
        currentTime = totalTime;
        timerStarted = false;
        timerEnded = false;
        UpdateTimerText();
    }

    public bool getTimerStarted()
	{
        return timerStarted;
	}

    private void setTimerToTime(int minutes, int seconds)
	{
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
