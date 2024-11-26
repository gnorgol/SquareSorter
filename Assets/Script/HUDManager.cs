using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI infoText;
    [SerializeField]
    private TextMeshProUGUI gameOverText;
    [SerializeField]
    int timerShowinfoText = 3;

    // coroutine
    private Coroutine coroutineShowInfo;

    private void OnEnable()
    {

        EventManager.OnGainScore += UpdateScoreDisplay;
        EventManager.OnTimerUpdate += UpdateTimerDisplay;
        EventManager.OnLevelUpdate += UpdateLevelDisplay;
        EventManager.OnInfoUpdate += UpdateInfoDisplay;
        EventManager.OnGameOver += ShowGameOver;
        EventManager.OnGameStart += GameStart;
    }
    private void OnDisable()
    {
        EventManager.OnGainScore -= UpdateScoreDisplay;
        EventManager.OnTimerUpdate -= UpdateTimerDisplay;
        EventManager.OnLevelUpdate -= UpdateLevelDisplay;
        EventManager.OnInfoUpdate -= UpdateInfoDisplay;
        EventManager.OnGameOver -= ShowGameOver;
        EventManager.OnGameStart -= GameStart;
    }

    private void Start()
    {
        scoreText.text = "Score: 0";
    }

    private void UpdateScoreDisplay(int newScore)
    {
        scoreText.text = "Score: " + newScore;

    }
    private void UpdateTimerDisplay(float newTime)
    {
        int minutes = Mathf.FloorToInt(newTime / 60);
        int seconds = Mathf.FloorToInt(newTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    private void UpdateLevelDisplay(int newLevel)
    {
        levelText.text = "Level: " + newLevel;
    }
    private void UpdateInfoDisplay(string newInfo)
    {
        //stop the coroutine if it's already running
        if (coroutineShowInfo != null)
        {
            StopCoroutine(coroutineShowInfo);
        }
        infoText.text = newInfo;
        coroutineShowInfo = StartCoroutine(ShowInfoText());
    }
    private IEnumerator ShowInfoText()
    {
        yield return new WaitForSeconds(timerShowinfoText);
        infoText.text = "";
    }
    private void ShowGameOver()
    {
        infoText.text = "";
        gameOverText.text = "Game Over";
        gameOverText.color = Color.red;
    }
    private void GameStart()
    {
        gameOverText.text = "";
    }
}
