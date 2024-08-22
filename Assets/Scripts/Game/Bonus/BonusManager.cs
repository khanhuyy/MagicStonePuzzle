using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public List<GameObject> bonusList;

    private void Start()
    {
        GameEvents.ShowBonusScreen += GameEvents_ShowBonusScreen;
    }
    
    private void OnDisable()
    {
        GameEvents.ShowBonusScreen -= GameEvents_ShowBonusScreen;
    }

    private void GameEvents_ShowBonusScreen(Config.TileColor color)
    {
        GameObject obj = null;
        foreach (var bonus in bonusList)
        {
            var bonusComp = bonus.GetComponent<Bonus>();
            if (bonusComp.color == color)
            {
                obj = bonus;
                bonus.SetActive(true);
            }
        }

        StartCoroutine(DeactivateBonus(obj));
    }

    private IEnumerator DeactivateBonus(GameObject obj)
    {
        yield return new WaitForSeconds(2f);
        obj.SetActive(false);
    }
}
