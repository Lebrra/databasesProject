using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;

    private void Awake()
    {
        if (instance)
            if (instance != this) instance.EnableScreen(false);

        instance = this;
        EnableScreen(false);
    }

    public void EnableScreen(bool enable)
    {
        gameObject.SetActive(enable);
    }
}
