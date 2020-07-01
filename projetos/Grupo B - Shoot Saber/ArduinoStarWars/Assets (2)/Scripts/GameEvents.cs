using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Current;
    public event Action PlayerDied;
    public event Action PlayerSurvived;
    public event Action PlayerCalibrated;

    public void PlayerLost()
    {
        if (PlayerDied != null)
        {
            PlayerDied();
        }
    }

    public void PlayerWon()
    {
        if (PlayerSurvived != null)
        {
            PlayerSurvived();
        }
    }

    public void PlayerIsReady()
    {
        if(PlayerCalibrated != null)
        {
            PlayerCalibrated();
        }
    }

    private void Awake()
    {
        Current = this;
    }
}
