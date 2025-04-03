using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    Image img;
    Color tempColor;

    void Start()
    {
        img = GetComponent<Image>();
        tempColor = img.color;
        tempColor.a = 1f;
        img.color = tempColor;
        StartCoroutine(FadeIn(0.15f));
    }

    public IEnumerator FadeIn(float seconds)
    {
        for (float a = 1f; a >= -0.05f; a -= 0.05f)
        {
            tempColor = img.color;
            tempColor.a = a;
            img.color = tempColor;
            yield return new WaitForSecondsRealtime(seconds);
        }
        img.raycastTarget = false;
    }

    public IEnumerator FadeOut(float seconds)
    {
        img.raycastTarget = true;
        for (float a = 0.05f; a <= 1.05f; a += 0.05f)
        {
            tempColor = img.color;
            tempColor.a = a;
            img.color = tempColor;
            yield return new WaitForSecondsRealtime(seconds);
        }

    }
}
