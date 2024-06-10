using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinLoseManager : MonoBehaviour
{
    public Canvas winCanvas;
    public Canvas loseCanvas;
    public Text winScoreText;
    public Text winHappyCustomersText;
    public Text winAngryCustomersText;
    public Text loseScoreText;
    public Text loseHappyCustomersText;
    public Text loseAngryCustomersText;

    public void PlayerWins(int score, int happyCustomers, int angryCustomers)
    {
        winCanvas.enabled = true;
        winScoreText.ChangeText(score.ToString());
        winHappyCustomersText.ChangeText(happyCustomers.ToString());
        winAngryCustomersText.ChangeText(angryCustomers.ToString());
    }

    public void PlayerLoses(int score, int happyCustomers, int angryCustomers)
    {
        loseCanvas.enabled = true;
        loseScoreText.ChangeText(score.ToString());
        loseHappyCustomersText.ChangeText(happyCustomers.ToString());
        loseAngryCustomersText.ChangeText(angryCustomers.ToString());
    }

    public void PlayerContinueOrRetry()
    {
        if (winCanvas.isActiveAndEnabled)
        {
            winCanvas.enabled = false;
        }
        else if (loseCanvas.isActiveAndEnabled)
        {
            loseCanvas.enabled = false;
        }
    }
}
