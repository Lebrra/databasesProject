using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SelectDevInfo()
    {
        // gonna call it 'auditing' like some official biz
        Debug.Log("Selected for audit: " + DevLoader.ddat.ToString());
        FrontDesk.ddat = DevLoader.ddat;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
