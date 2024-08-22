using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class TileTextureData : ScriptableObject
{
    [System.Serializable]
    public class TextureData
    {
        public Sprite texture;
        public Config.TileColor tileColor;
    }

    public int tresholdValue = 10;
    private const int StartTresholdValue = 50;
    public List<TextureData> activeTileTextures;
    
    public Config.TileColor currentColor;
    private Config.TileColor nextColor;

    public int GetCurrentColorIndex()
    {
        var currentIndex = 0;
        for (int index = 0; index < activeTileTextures.Count; index++)
        {
            if (activeTileTextures[index].tileColor == currentColor)
            {
                currentIndex = index;
            }
        }

        return currentIndex;
    }

    public void UpdateColors(int currentScore)
    {
        currentColor = nextColor;
        var currentColorIndex = GetCurrentColorIndex();
        if (currentColorIndex == activeTileTextures.Count - 1)
        {
            nextColor = activeTileTextures[0].tileColor;
        }
        else
        {
            nextColor = activeTileTextures[currentColorIndex + 1].tileColor;
        }

        tresholdValue = StartTresholdValue + currentScore;
    }
    
    public void SetStartColor()
    {
        tresholdValue = StartTresholdValue;
        currentColor = activeTileTextures[0].tileColor;
        nextColor = activeTileTextures[1].tileColor;
    }

    private void Awake()
    {
        SetStartColor();
    }

    private void OnEnable()
    {
        SetStartColor();
    }
}
