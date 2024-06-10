using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public float TimerValue;
    public UnityEvent<string> UpdateText;
    public UnityEvent<string> UpdateSeconds;
    public UnityEvent<string> UpdateMinutes;
    public UnityEvent OnTimerStart;
    public UnityEvent OnTimerOver;

    public float ExtraTime;
    private float TimeReference;
    private float FirstTimeReference;
    private bool isCountdown = false;

    [HideInInspector]public float Value;

    private int MinuteSecondsValue;

    private void Update()
    {
        if (!isCountdown)
            return;

        Value = Mathf.Clamp((TimerValue + ExtraTime) - (Time.time - TimeReference), 0, TimerValue + ExtraTime);

        if(MinuteSecondsValue != Mathf.FloorToInt(Value))
        {
            MinuteSecondsValue = Mathf.FloorToInt(Value);
            UpdateSeconds?.Invoke(GetSeconds());
            UpdateMinutes?.Invoke(GetMinutes());
        }

        if (UpdateText != null)
        {
            UpdateText.Invoke(GetTimeInSeconds());
        }

        if (Value > 0) return;

        TimerValue = 0;

        TimerEnd();
    }

    private void TimerEnd()
    {
        isCountdown = false;

        if (OnTimerOver != null)
        {
            OnTimerOver.Invoke();
        }
    }

    public void PreSetTimmer()
    {
        Value = TimerValue;
        if (UpdateText != null)
        {
            UpdateText.Invoke(GetTimeInSeconds());
        }
    }

    public void SetTimer()
    {
        TimeReference = Time.time;
        FirstTimeReference = Time.time;
        isCountdown = true;
        OnTimerStart?.Invoke();
    }

    public void RestartTimer()
    {
        TimeReference = Time.time;
    }

    public void PauseTimer()
    {
        isCountdown = false;
    }

    public void ResumeTimer()
    {
        TimeReference = Time.time + (TimeReference - FirstTimeReference);
        FirstTimeReference = TimeReference;
        isCountdown = true;
    }

    public void AddTime(float value)
    {
        ExtraTime += value;
    }

    public void RestTime(float value)
    {
        ExtraTime -= value;
    }

    public float GetTimeInFloat()
    {
        return Value;
    }

    public string GetTimeInSeconds()
    {
        int minutos = Mathf.FloorToInt(Value / 60);
        int segundos = Mathf.FloorToInt(Value % 60);

        return string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    public string GetSeconds()
    {
        int segundos = Mathf.FloorToInt(Value % 60);

        return segundos.ToString("00");
    }

    public string GetMinutes()
    {
        int minutos = Mathf.FloorToInt(Value / 60);

        return minutos.ToString("00");
    }

    public int GetRealTime()
    {
        return (int)Mathf.Clamp((TimerValue) - (Time.time - TimeReference), 0, TimerValue);
    }
}
