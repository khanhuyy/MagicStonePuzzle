using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBar : MonoBehaviour
{
    public Image fillInImage;
    public TextMeshProUGUI bestScoreText;
    
    private void OnEnable()
    {
        GameEvents.UpdateBestScoreBar += GameEvents_UpdateBestScoreBar;
    }

    private void OnDisable()
    {
        GameEvents.UpdateBestScoreBar -= GameEvents_UpdateBestScoreBar;
    }

    private void GameEvents_UpdateBestScoreBar(int currentScore, int bestScore)
    {
        float currentPercentage = (float) currentScore / (bestScore == 0 ? 1 : bestScore);
        fillInImage.fillAmount = currentPercentage > 1 ? 1 : currentPercentage;
        bestScoreText.text = bestScore.ToString();
    }
    
}
