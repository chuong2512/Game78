using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginReward : MonoBehaviour
{
    private int _currentDay;
    private bool _isReceived = true;

    public GameObject buttonReward;
    
    void Start()
    {
        ResetData();
        
        buttonReward.SetActive(!_isReceived);
    }

    private void ResetData()
    {
        _currentDay = PlayerPrefs.GetInt("LoginDay", 0);
        var lastTime = PlayerPrefs.GetFloat("TimeLogin", 0);

        var currentTime = DateTime.Now.DayOfYear;

        if (currentTime > lastTime)
        {
            _isReceived = false;
            _currentDay++;
        }
    }

    public void ClickCollect()
    {
        if (!_isReceived)
        {
            PlayerPrefs.SetInt("LoginDay", _currentDay);
            PlayerPrefs.SetFloat("TimeLogin", DateTime.Now.DayOfYear);

            GlobalValue.Coin += 10;
            
            _isReceived = true;
            buttonReward.SetActive(!_isReceived);
        }
    }
}