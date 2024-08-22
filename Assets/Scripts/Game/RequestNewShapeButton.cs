using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RequestNewShapeButton : MonoBehaviour
{
    public int requestQuantity = 3;
    public TextMeshProUGUI quantityText;

    private int _currentRequestQuantity;
    private Button _button;
    private bool _isLocked;
    
    private void Start()
    {
        _currentRequestQuantity = requestQuantity;
        quantityText.text = _currentRequestQuantity.ToString();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonDown);
        UnLock();
    }

    private void OnButtonDown()
    {
        if (!_isLocked)
        {
            _currentRequestQuantity--;
            GameEvents.RequestNewShapes();
            GameEvents.CheckIfPlayerLose();
            if (_currentRequestQuantity <= 0)
            {
                Lock() ;
            }

            quantityText.text = _currentRequestQuantity.ToString();
        }
    }

    private void Lock()
    {
        _isLocked = true;
        _button.interactable = false;
        quantityText.text = _currentRequestQuantity.ToString();
    }
    
    private void UnLock()
    {
        _isLocked = false;
        _button.interactable = true;
    }
}
