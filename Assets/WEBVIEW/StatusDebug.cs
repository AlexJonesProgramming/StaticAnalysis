using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusDebug : MonoBehaviour
{

    public Text status;
    private string previousStatus = "";
    public string closeCondition = "CallOnLoaded[https://thermofisher.my.idaptive.app/login]";

    // Update is called once per frame
    void Update()
    {
        if(status.text != previousStatus)
        {
            previousStatus = status.text;
            Debug.LogError(status.text);
        }

        if(status.text == closeCondition)
        {

            Debug.LogError("TRIED TO TURN OFF THE WEBVIEW");
            this.gameObject.SetActive(false);
        }
    }
}
