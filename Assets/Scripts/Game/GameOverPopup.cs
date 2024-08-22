using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameOverPopup : MonoBehaviour
{
    public GameObject gameOverPopup;
    public GameObject losePopup;
    public GameObject newBestScorePopup;
    public TextMeshProUGUI gameOverScoreText;
    public Score currentScore;
    
    private void Start()
    {
        gameOverPopup.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.GameOver += GameEvents_GameOver;
    }
    
    private void OnDisable()
    {
        GameEvents.GameOver -= GameEvents_GameOver;
    }

    private void GameEvents_GameOver(bool newBestScore)
    {
        gameOverPopup.SetActive(true);
        losePopup.SetActive(true);
        newBestScorePopup.SetActive(true);
        gameOverScoreText.text = currentScore.currentScores.ToString();
    }
}
