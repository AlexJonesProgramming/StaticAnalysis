using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRects : MonoBehaviour
{
    public RectTransform rectTransform;
    public Crystal.SafeArea safeArea;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        int i = 100;
        while(i > 0)
        {
            i--;
            Debug.LogError(safeArea.GetSafeArea());

            Rect rect = safeArea.GetSafeArea();

            rect.xMax -= Screen.width;
            rect.yMax -= Screen.height;

            Debug.LogError(rect.xMin + " " + rect.yMin + " " + (rect.xMax) + " " + (rect.yMax));


            yield return new WaitForEndOfFrame();
        }

        //rectTransform.rect = safeArea.GetSafeArea();
    }
}
