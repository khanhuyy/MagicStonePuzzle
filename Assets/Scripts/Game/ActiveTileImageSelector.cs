using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveTileImageSelector : MonoBehaviour
{
    public TileTextureData tileTextureData;
    public bool updateImageOnReachTreshold = false;

    private void OnEnable()
    {
        UpdateTileColorBaseOnCurrentScores();
        if (updateImageOnReachTreshold)
        {
            GameEvents.UpdateTilesColor += GameEvents_UpdateTilesColor;
        }
    }

    private void OnDisable()
    {
        if (updateImageOnReachTreshold)
        {
            GameEvents.UpdateTilesColor -= GameEvents_UpdateTilesColor;
        }
    }

    private void UpdateTileColorBaseOnCurrentScores()
    {
        foreach (var tileTexture in tileTextureData.activeTileTextures)
        {
            if (tileTextureData.currentColor == tileTexture.tileColor)
            {
                GetComponent<Image>().sprite = tileTexture.texture;
            }
        }
    }

    private void GameEvents_UpdateTilesColor(Config.TileColor color)
    {
        foreach (var tile in tileTextureData.activeTileTextures)
        {
            if (color == tile.tileColor)
            {
                GetComponent<Image>().sprite = tile.texture;
            }
        }
    }
}
