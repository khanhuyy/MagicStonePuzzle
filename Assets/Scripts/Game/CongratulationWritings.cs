using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongratulationWritings : MonoBehaviour
{
    public List<GameObject> writings;
    
    void Start()
    {
        GameEvents.ShowCongratulationWritings += GameEvents_ShowCongratulationWritings;
    }

    private void OnDisable()
    {
        
        
    }

    private void GameEvents_ShowCongratulationWritings()
    {
        var index = UnityEngine.Random.Range(0, writings.Count);
        writings[index].SetActive(true);
    }
}
