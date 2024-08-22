using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int columns = 0;
    public int rows = 0;
    public float squaresGap = 0.1f;
    public GameObject gridSquare;
    public Vector2 startPosition;
    public float tileScale = 0.5f;
    public float everySquareOffset = 0.0f;
    public TileTextureData tileTextureData;

    private Vector2 _offset = Vector2.zero;
    private List<GameObject> _gridSquares;

    private LineIndicator lineIndicator;
    private Config.TileColor currentTileColor = Config.TileColor.NotSet;
    private List<Config.TileColor> colorsInGrid = new List<Config.TileColor>();

    [SerializeField] private GridSoundEffect _sfx;
    
    void Start()
    {
        lineIndicator = GetComponent<LineIndicator>();
        _gridSquares = new List<GameObject>();
        CreateGrid();
        currentTileColor = tileTextureData.activeTileTextures[0].tileColor;
    }

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += GameEvents_CheckIfShapeCanBePlaced;
        GameEvents.UpdateTilesColor += GameEvents_UpdateTilesColor;
        GameEvents.CheckIfPlayerLose += HaveAnyValidMove; // todo refactor
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= GameEvents_CheckIfShapeCanBePlaced;
        GameEvents.UpdateTilesColor -= GameEvents_UpdateTilesColor;
        GameEvents.CheckIfPlayerLose -= HaveAnyValidMove;
    }

    private void GameEvents_CheckIfShapeCanBePlaced()
    {
        var squareIndexes = new List<int>();
        foreach (var square in _gridSquares)
        {
            var component = square.GetComponent<GridSquare>();
            if (component.Selected && !component.SquareOccupied)
            {
                squareIndexes.Add(component.SquareIndex);
                component.Selected = false;
                // component.ActivateSquare();
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null)
            return;
        if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count)
        {
            foreach (var squareIndex in squareIndexes)
            {
                _gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard(currentTileColor);
            }

            var shapeLeft = 0;

            foreach (var shape in shapeStorage.shapeList)
            {
                if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    shapeLeft++;
                }
            }
            if (shapeLeft == 0)
            {
                GameEvents.RequestNewShapes();
            }
            else
            {
                GameEvents.SetShapeInactive();
            }
            _sfx.PlayPlaceOnGridsSound();
            CheckFullLine();
        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }
    }

    private void GameEvents_UpdateTilesColor(Config.TileColor color)
    {
        currentTileColor = color;
    }

    private List<Config.TileColor> GetAllTileColorsInGrid()
    {
        var colors = new List<Config.TileColor>();
        foreach (var tile in _gridSquares)
        {
            var gridTile = tile.GetComponent<GridSquare>();
            if (gridTile.SquareOccupied)
            {
                var color = gridTile.GetCurrentColor();
                if (!colors.Contains(color))
                {
                    colors.Add(color);
                }
            }
        }

        return colors;
    }
    
    void CheckFullLine()
    {
        List<int[]> lines = new List<int[]>();
        
        // columns
        foreach (var column in lineIndicator.columnIndexes)
        {
            lines.Add(lineIndicator.GetVerticalLine(column));
        }
        
        // rows
        for (int row = 0; row < 9; row++)
        {
            List<int> data = new List<int>(9);
            for (int index = 0; index < 9; index++)
            {
                data.Add(lineIndicator.lineData[row, index]);
            }
            lines.Add(data.ToArray());
        }

        // 3x3 squares
        for (int square = 0; square < 9; square++)
        {
            List<int> data = new List<int>(9);
            for (int index = 0; index < 9; index++)
            {
                data.Add(lineIndicator.squareData[square, index]);
            }
            lines.Add(data.ToArray());
        }

        colorsInGrid = GetAllTileColorsInGrid();
        
        // bonus
        var completeLines = GetCompletedLineAndCompleteTiles(lines);
        if (completeLines >= 2)
        {
            GameEvents.ShowCongratulationWritings();
        }

        var totalScores = 10 * (int)Math.Pow(2, completeLines);
        // todo add color bonus
        // var bonusColorScore = ShouldPlayColorBonusAnimation();
        GameEvents.AddScores(totalScores);
        GameEvents.CheckIfPlayerLose();
        // HaveAnyValidMove(); // validate lose state
    }

    private int ShouldPlayColorBonusAnimation()
    {
        var colorInTheFridAfterLineRemoved = GetAllTileColorsInGrid();
        Config.TileColor colorToPlayBonusFor = Config.TileColor.NotSet;
        foreach (var tileColor in colorsInGrid)
        {
            if (!colorInTheFridAfterLineRemoved.Contains(tileColor))
            {
                colorToPlayBonusFor = tileColor;
            }
        }

        if (colorToPlayBonusFor == Config.TileColor.NotSet || colorToPlayBonusFor == currentTileColor)
        {
            Debug.Log("Cannot find color for bonus or the same color current tier");
            return 0;
        }

        GameEvents.ShowBonusScreen(colorToPlayBonusFor);
        return 50; // todo export bonus point constant
    }
    
    private int GetCompletedLineAndCompleteTiles(List<int[]> data)
    {
        List<int[]> completeLines = new List<int[]>();
        var linesCompleted = 0;
        foreach (var line in data)
        {
            var lineCompleted = true;
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                if (!comp.SquareOccupied)
                {
                    lineCompleted = false;
                }
            }

            if (lineCompleted)
            {
                completeLines.Add(line);
            }
        }
        
        foreach (var line in completeLines)
        {
            var completed = false;
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.Deactivate();
                completed = true;
            }
            // ?? split for what
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if (completed)
            {
                linesCompleted++;
            }
        }

        return linesCompleted;
    }
    
    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridTilesPositions();
    }
    
    private void SpawnGridSquares()
    {
        // 0, 1, 2
        // 3, 4, 5
        int squareIndex = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                _gridSquares.Add(Instantiate(gridSquare, transform));
                _gridSquares[^1].transform.localScale =
                    new Vector3(tileScale, tileScale, tileScale);
                if (_gridSquares[^1].TryGetComponent(out GridSquare square))
                {
                    square.SetImage(lineIndicator.GetGridSquareIndex(squareIndex) % 2 == 0);
                    square.SquareIndex = squareIndex;
                }
                squareIndex++;
            }
        }
    }

    private void SetGridTilesPositions()
    {
        int columnNumber = 0;
        int rowNumber = 0;
        Vector2 tileGapNumber = Vector2.zero;
        bool rowMoved = false;

        var squareRect = _gridSquares[0].GetComponent<RectTransform>();

        _offset.x = squareRect.rect.width * squareRect.transform.localScale.x + everySquareOffset;
        _offset.y = squareRect.rect.height * squareRect.transform.localScale.y + everySquareOffset;

        foreach (var square in _gridSquares)
        {
            if (columnNumber + 1 > columns)
            {
                tileGapNumber.x = 0;
                columnNumber = 0;
                rowNumber++;
                rowMoved = true;
            }

            var posXOffset = _offset.x * columnNumber + (tileGapNumber.x * squaresGap);
            var posYOffset = _offset.y * rowNumber + (tileGapNumber.y * squaresGap);

            if (columnNumber > 0 && columnNumber % 3 == 0)
            {
                tileGapNumber.x++;
                posXOffset += squaresGap;
            }

            if (rowNumber > 0 && rowNumber % 3 == 0 && !rowMoved)
            {
                rowMoved = true;
                tileGapNumber.y++;
                posYOffset += squaresGap;
            }

            square.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(startPosition.x + posXOffset, startPosition.y - posYOffset);
            
            square.GetComponent<RectTransform>().localPosition =
                new Vector3(startPosition.x + posXOffset, startPosition.y - posYOffset, 0f);
            columnNumber++;
        }
    }

    private void HaveAnyValidMove()
    {
        var validShape = 0;
        for (int index = 0; index < shapeStorage.shapeList.Count; index++)
        {
            var isShapeActive = shapeStorage.shapeList[index].IsAnyOfShapeSquareActive();
            if (CheckIfShapeCanBePlaceOnGrid(shapeStorage.shapeList[index]) && isShapeActive)
            {
                shapeStorage.shapeList[index]?.ActivateShape();
                validShape++;
            }
        }

        if (validShape == 0)
        {
            // game over
            GameEvents.GameOver(false);
        }
    }

    private bool CheckIfShapeCanBePlaceOnGrid(Shape currentShape)
    {
        var currentShapeData = currentShape.currentShapeData;
        var shapeColumns = currentShapeData.columns;
        var shapeRows = currentShapeData.rows;
        
        // all indexes of filled up squares.
        List<int> originalShapeFilledUpTiles = new List<int>();
        var tileIndex = 0;
        for (int rowIndex = 0; rowIndex < shapeRows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < shapeColumns; columnIndex++)
            {
                if (currentShapeData.board[rowIndex].column[columnIndex])
                {
                    originalShapeFilledUpTiles.Add(tileIndex);
                }
                tileIndex++;
            }
        }
        
        if (currentShape.TotalSquareNumber != originalShapeFilledUpTiles.Count)
            Debug.LogError("Number of filled up square are not the same as the original shape have.");
        var tileList = GetAllTilesCombination(shapeColumns, shapeRows);
        bool canBePlaced = false;
        foreach (var tile in tileList)
        {
            bool shapeCanBePlacedOnTheBoard = true;
            foreach (var originalTile in originalShapeFilledUpTiles)
            {
                var comp = _gridSquares[tile[originalTile]].GetComponent<GridSquare>();
                if (comp.SquareOccupied)
                {
                    shapeCanBePlacedOnTheBoard = false;
                }
            }

            if (shapeCanBePlacedOnTheBoard)
                canBePlaced = true;
        }
        return canBePlaced;
    }

    private List<int[]> GetAllTilesCombination(int shapeColumns, int shapeRows)
    {
        var tileList = new List<int[]>();
        var lastColumnIndex = 0;
        var lastRowIndex = 0;
        int safeIndex = 0;

        while (lastRowIndex + (shapeRows - 1) < 9)
        {
            var rowData = new List<int>();

            for (int row = lastRowIndex; row < lastRowIndex + shapeRows; row++)
            {
                for (int column = lastColumnIndex; column < lastColumnIndex + shapeColumns; column++)
                {
                    rowData.Add(lineIndicator.lineData[row, column]);
                }
            }
            
            tileList.Add(rowData.ToArray());
            lastColumnIndex++;
            if (lastColumnIndex + (shapeColumns - 1) >= 9)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safeIndex++;
            if (safeIndex > 100)
                break;
        }

        return tileList;
    }
}
