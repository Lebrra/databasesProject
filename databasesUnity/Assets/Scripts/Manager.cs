using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    void Start()
    {
        SQLConnection.Connect();

        SQLConnection.testSQL();
    }


    private void OnApplicationQuit()
    {
        SQLConnection.EndConnection();
    }
}
