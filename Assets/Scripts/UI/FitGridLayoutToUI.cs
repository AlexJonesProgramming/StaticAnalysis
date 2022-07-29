using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FitGridLayoutToUI : MonoBehaviour
{
    public GridLayoutGroup gridLayout;
    public RectTransform container;

    //The values we need to know from the layout group
    int gridCellWidth, gridCellHeight;
    int gridCellSpacingWidth, gridCellSpacingHeight;

    float ratioX, ratioY;


    int containerWidth;



    private void Start()
    {
        StartCoroutine(InitCo());
    }

    IEnumerator InitCo()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Init();
    }


    private void Init()
    {
        gridCellWidth = (int)gridLayout.cellSize.x;
        gridCellHeight = (int)gridLayout.cellSize.y;
        gridCellSpacingWidth = (int)gridLayout.spacing.x;
        gridCellSpacingHeight = (int)gridLayout.spacing.y;

        int gcd = GCD(gridCellWidth, gridCellHeight);

        ratioX = gridCellWidth / gcd;
        ratioY = gridCellHeight / gcd;

        //Get the width of the container
        containerWidth = (int)container.rect.width;

        //Figure out how much space we will have after the cell spacing
        containerWidth = containerWidth - (gridCellSpacingWidth * 4);

        //The width for each card
        containerWidth = containerWidth / 3;
        int cardHeight = (int)((containerWidth / ratioX) * ratioY);

        gridLayout.cellSize = new Vector2(containerWidth, cardHeight);

        //Turning it off and back on again seems to be the only way to update these values
        gridLayout.enabled = false;
        gridLayout.enabled = true;
    }


    public int GCD(int a, int b)
    {
        while (a != 0 && b != 0)
        {
            if (a > b)
                a %= b;
            else
                b %= a;
        }
        if (a == 0)
            return b;
        else
            return a;
    }

}
