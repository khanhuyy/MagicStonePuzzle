using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GameObject squareShapeImage;
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new Vector2(0, 700);

    [HideInInspector]
    public ShapeData currentShapeData;

    public int TotalSquareNumber { get; set; }
    private List<GameObject> _currentTiles; // total tile in this shape
    private Vector3 _shapeStartScale;
    private RectTransform _transform;
    private bool _shapeDraggable;
    private Canvas _canvas;
    private Vector3 _startPosition;
    private bool _shapeActive;

    private void Awake()
    {
        _currentTiles = new List<GameObject>();
        _shapeStartScale = GetComponent<RectTransform>().localScale;
        _transform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _shapeDraggable = true;
        _startPosition = _transform.localPosition;
        _shapeActive = true;
    }

    private void OnEnable()
    {
        GameEvents.MoveShapeToStartPosition += GameEvents_MoveShapeToStartPosition;
        GameEvents.SetShapeInactive += GameEvents_SetShapeInactive;
    }

    private void OnDisable()
    {
        GameEvents.MoveShapeToStartPosition -= GameEvents_MoveShapeToStartPosition;
        GameEvents.SetShapeInactive -= GameEvents_SetShapeInactive;
    }

    private void GameEvents_MoveShapeToStartPosition()
    {
        _transform.transform.localPosition = _startPosition;
    }

    private void GameEvents_SetShapeInactive()
    {
        if (!IsOnStartPosition() && IsAnyOfShapeSquareActive())
        {
            foreach (var tile in _currentTiles)
            {
                tile.gameObject.SetActive(false);
            }
        }
        // todo sound here
    }
    
    public void ActivateShape()
    {
        if (!_shapeActive)
        {
            foreach (var square in _currentTiles)
            {
                square?.GetComponent<ShapeSquare>().ActivateTile();
            }
        }

        _shapeActive = true;
    }
    
    public void DeactivateShape()
    {
        if (_shapeActive)
        {
            foreach (var square in _currentTiles)
            {
                square?.GetComponent<ShapeSquare>().DeactivateTile();
            }
        }

        _shapeActive = false;
    }
    
    public bool IsOnStartPosition()
    {
        return _transform.localPosition == _startPosition;
    }

    public bool IsAnyOfShapeSquareActive()
    {
        foreach (var square in _currentTiles)
        {
            if (square.gameObject.activeSelf)
                return true;
        }

        return false;
    }

    private void Start()
    {
        // RequestNewShape(CurrentShapeData);
    }

    public void RequestNewShape(ShapeData shapeData)
    {
        _transform.localPosition = _startPosition;
        CreateShape(shapeData);
    }
    
    public void CreateShape(ShapeData shapeData)
    {
        currentShapeData = shapeData;
        TotalSquareNumber = GetNumberOfSquare(shapeData);
        while (_currentTiles.Count <= TotalSquareNumber)
        {
            _currentTiles.Add(Instantiate(squareShapeImage, transform) as GameObject);
        }

        foreach (var square in _currentTiles)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = squareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x,
            squareRect.rect.height * squareRect.localScale.y);

        int currentIndexInList = 0;
        for (int row = 0; row < shapeData.rows; row++)
        {
            for (int column = 0; column < shapeData.columns; column++)
            {
                if (shapeData.board[row].column[column])
                {
                    _currentTiles[currentIndexInList].SetActive(true);
                    _currentTiles[currentIndexInList].GetComponent<RectTransform>().localPosition = new Vector2(
                        GetXPositionForShapeSquare(shapeData, column, moveDistance),
                        GetYPositionForShapeSquare(shapeData, row, moveDistance));
                    currentIndexInList++;
                }
            }
        }
    }

    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0f;
        if (shapeData.rows > 1)
        {
            if (shapeData.rows % 2 != 0)
            {
                var middleSquareIndex = (shapeData.rows - 1) / 2;
                var multiplier = (shapeData.rows - 1) / 2;
                if (row < middleSquareIndex) // move it on the negative
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }
                else if (row > middleSquareIndex) // move on positive
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY *= multiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.rows == 2) ? 1 : (shapeData.rows / 2);
                var middleSquareIndex1 = (shapeData.rows == 2) ? 0 : (shapeData.rows - 1);
                var multiplier = shapeData.rows / 2;

                if (row == middleSquareIndex1 || row == middleSquareIndex2)
                {
                    if (row == middleSquareIndex2)
                        shiftOnY = moveDistance.y / 2 * -1;
                    if (row == middleSquareIndex1)
                        shiftOnY = moveDistance.y / 2;
                }
                if (row < middleSquareIndex1 && row < middleSquareIndex2) // move on negative
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY *= multiplier;
                }
                else if (row > middleSquareIndex1 && row > middleSquareIndex2) // move to positive
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY += multiplier;
                }
            }
        }

        return shiftOnY;
    }

    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        float shiftOnX = 0f;
        if (shapeData.columns > 1)
        {
            if (shapeData.columns % 2 != 0)
            {
                var middleSquareIndex = (shapeData.columns - 1) / 2;
                var multiplier = (shapeData.columns - 1) / 2;
                if (column < middleSquareIndex) // move it on the negative
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIndex) // move on positive
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX *= multiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.columns == 2) ? 1 : (shapeData.columns / 2);
                var middleSquareIndex1 = (shapeData.columns == 2) ? 0 : (shapeData.columns - 1);
                var multiplier = shapeData.columns / 2;

                if (column == middleSquareIndex1 || column == middleSquareIndex2)
                {
                    if (column == middleSquareIndex2)
                        shiftOnX = moveDistance.x / 2;
                    if (column == middleSquareIndex1)
                        shiftOnX = moveDistance.x / 2 * -1;
                }
                if (column < middleSquareIndex1 && column < middleSquareIndex2) // move on negative
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIndex1 && column > middleSquareIndex2) // move to positive
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX += multiplier;
                }
            }
        }

        return shiftOnX;
    }

    private int GetNumberOfSquare(ShapeData shapeData)
    {
        int number = 0;
        foreach (var rowData in shapeData.board)
        {
            foreach (var active in rowData.column)
            {
                if (active)
                    number++;
            }
        }

        return number;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<RectTransform>().localScale = shapeSelectedScale;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchorMin = Vector2.one / 2;
        _transform.anchorMax = Vector2.one / 2;
        _transform.pivot = Vector2.one / 2;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, eventData.position,
            Camera.main, out pos);
        _transform.localPosition = pos + offset;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<RectTransform>().localScale = _shapeStartScale;
        GameEvents.CheckIfShapeCanBePlaced();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}
