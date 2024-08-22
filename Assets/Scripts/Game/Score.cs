using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class BestScoreData
{
    public int score = 0;
}

public class Score : MonoBehaviour
{
    public TileTextureData tileTextureData;
    public TextMeshProUGUI scoreText;

    private bool newBestScore;
    private BestScoreData bestScore = new BestScoreData();
    public int currentScores;

    private string bestScoreKey = "bsdat"; // Best Score DATa
    
    private void Awake()
    {
        if (BinaryDataStream.Exist(bestScoreKey))
        {
            StartCoroutine(ReadDataFile());
        }
    }

    private IEnumerator ReadDataFile()
    {
        bestScore = BinaryDataStream.Read<BestScoreData>(bestScoreKey);
        yield return new WaitForEndOfFrame();
        GameEvents.UpdateBestScoreBar(currentScores, bestScore.score);
        Debug.Log("best score" + bestScore.score);
    }
    

    private void Start()
    {
        currentScores = 0;
        newBestScore = false;
        tileTextureData.SetStartColor();
        UpdateScoreText();
    }

    private void OnEnable()
    {
        GameEvents.AddScores += GameEvents_AddScore;
        GameEvents.GameOver += GameEvents_GameOver;
    }
    
    private void OnDisable()
    {
        GameEvents.AddScores -= GameEvents_AddScore;
        GameEvents.GameOver -= GameEvents_GameOver;
    }

    private void GameEvents_AddScore(int score)
    {
        currentScores += score;
        if (currentScores > bestScore.score)
        {
            newBestScore = true;
            bestScore.score = currentScores;
            GameEvents_GameOver(true);
        }
        UpdateTileColor();
        GameEvents.UpdateBestScoreBar(currentScores, bestScore.score);
        UpdateScoreText();
    }

    private void UpdateTileColor()
    {
        if (GameEvents.UpdateTilesColor != null && currentScores >= tileTextureData.tresholdValue)
        {
            tileTextureData.UpdateColors(currentScores);
            GameEvents.UpdateTilesColor(tileTextureData.currentColor);
        }
    }
    
    private void GameEvents_GameOver(bool isNewScore) // Save Best Score, todo rename event
    {
        BinaryDataStream.Save<BestScoreData>(bestScore, bestScoreKey);
    }

    private void UpdateScoreText()
    {
        scoreText.text = currentScores.ToString();
    }
}
