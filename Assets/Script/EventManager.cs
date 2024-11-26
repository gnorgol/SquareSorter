using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStart;
    public static event GameDelegate OnGameOver;
    public delegate void ScoreDelegate(int score);
    public static event ScoreDelegate OnGainScore;
    public delegate void TimerDelegate(float time);
    public static event TimerDelegate OnTimerUpdate;
    public delegate void LevelDelegate(int level);
    public static event LevelDelegate OnLevelUpdate;
    public delegate void InfoDelegate(string info);
    public static event InfoDelegate OnInfoUpdate;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void TriggerGameStart()
    {
        if (OnGameStart != null)
        {
            OnGameStart();
        }
    }
    public void TriggerGameOver()
    {
        if (OnGameOver != null)
        {
            OnGameOver();
            
        }
    }
    public void TriggerScore(int score)
    {
        if (OnGainScore != null)
        {
            OnGainScore(score);
        }
    }
    public void TriggerTimer(float time)
    {
        if (OnTimerUpdate != null)
        {
            OnTimerUpdate(time);
        }
    }
    public void TriggerLevel(int level)
    {
        if (OnLevelUpdate != null)
        {
            OnLevelUpdate(level);
        }
    }
    public void TriggerInfo(string info)
    {
        if (OnInfoUpdate != null)
        {
            OnInfoUpdate(info);
        }
    }


}
