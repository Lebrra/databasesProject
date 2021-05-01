using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DevButton : MonoBehaviour, IPointerClickHandler
{
    public int devID = -1;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (devID > 0)
        {
            DataUIPooler.poolInstance.GetDevPanel(devID);
        }
    }
}
