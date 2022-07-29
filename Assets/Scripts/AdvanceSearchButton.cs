using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AdvanceSearchButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public GameObject AdvanceSearchArea;

    //What the button should say when the user CANNOT see the advance search options
    string closedText = "Show Advanced Search Options +";
    //What the button should say when the user CAN see the advance search options
    string openedText = "Hide Advanced Search Options -";

    bool open = false;



    public void Toggle()
    {
        if (open)
        {
            open = false;
            AdvanceSearchArea.SetActive(false);
            buttonText.text = closedText;

        }
        else
        {
            open = true;
            AdvanceSearchArea.SetActive(true);
            buttonText.text = openedText;
        }
    }

}
