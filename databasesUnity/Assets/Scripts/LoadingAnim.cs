using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingAnim : MonoBehaviour
{
    public float speed = 1F;

    TextMeshProUGUI text;
    string myText;
    int current;

    void OnEnable()
    {
        text = GetComponent<TextMeshProUGUI>();
        myText = text.text.Trim('.');
        current = 0;

        StartCoroutine(LoadingText());
    }


    IEnumerator LoadingText()
    {
        yield return new WaitForSecondsRealtime(speed);

        if(current >= 3)
        {
            current = 0;
            myText = myText.Trim('.');
        }
        else
        {
            myText = myText + ".";
            current++;
        }
        text.text = myText;

        StartCoroutine(LoadingText());
    }
}
