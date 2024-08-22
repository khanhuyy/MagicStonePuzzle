using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image normalImage;
    public Image hoverImage;
    public Image activeImage;
    public List<Sprite> normalImages;

    private Config.TileColor currentTileColor = Config.TileColor.NotSet;

    public Config.TileColor GetCurrentColor()
    {
        return currentTileColor;
    }
    
    public bool Selected { get; set; }
    public int SquareIndex { get; set; }
    public bool SquareOccupied { get; set; }
    

    // Start is called before the first frame update

    void Start()
    {
        Selected = false;
        SquareOccupied = false;
    }

    public bool CanUseThisSquare()
    {
        return hoverImage.gameObject.activeSelf;
    }

    public void PlaceShapeOnBoard(Config.TileColor color)
    {
        currentTileColor = color;
        ActivateSquare();
    }

    public void ActivateSquare()
    {
        hoverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        Selected = true;
        SquareOccupied = true;
    }

    public void Deactivate()
    {
        currentTileColor = Config.TileColor.NotSet;
        activeImage.gameObject.SetActive(false);
    }

    public void ClearOccupied()
    {
        currentTileColor = Config.TileColor.NotSet;
        Selected = false;
        SquareOccupied = false;
    }
    
    public void SetImage(bool setFirstImage)
    {
        normalImage.sprite = setFirstImage ? normalImages[1] : normalImages[0];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!SquareOccupied)
        {
            Selected = true;
            hoverImage.gameObject.SetActive(true);
        }
        else if (other.GetComponent<ShapeSquare>() != null)
        {
            other.GetComponent<ShapeSquare>().SetOccupied();
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        Selected = true;
        if (!SquareOccupied)
        {
            hoverImage.gameObject.SetActive(true);
        }
        else if (other.GetComponent<ShapeSquare>() != null)
        {
            other.GetComponent<ShapeSquare>().SetOccupied();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!SquareOccupied)
        {
            Selected = false;
            hoverImage.gameObject.SetActive(false);
        }
        else if (other.GetComponent<ShapeSquare>() != null)
        {
            other.GetComponent<ShapeSquare>().UnSetOccupied();
        }
    }
}
