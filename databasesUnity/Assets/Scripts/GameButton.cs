using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameButton : MonoBehaviour, IPointerClickHandler
{
    public int gameRank = -1;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameRank > 0)
        {
            DataUIPooler.poolInstance.GetGamePanel(gameRank);
        }
    }
}
