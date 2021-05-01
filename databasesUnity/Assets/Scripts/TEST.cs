using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    void Start()
    {
        Invoke("TestQuery", 2F);
    }

    public void TestQuery()
    {
        DataUIPooler.poolInstance?.GetGamePanel(12);
    }
}
