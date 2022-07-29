using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTester : MonoBehaviour
{
    public void TestOpenConfirmDialogue()
    {
        TFEvents.OpenConfirmDialogue.Invoke("This is a headder", "This is a bunch of text it sure is just alot of text holding info and stuff yup.");
    }
    

    public void TestOpenAcceptDialogue()
    {
        TFEvents.OpenAcceptDialogue.Invoke
            (
                "This is a headder", 
                "This is a bunch of text it sure is just alot of text holding info and stuff yup.",
                () => { Debug.LogError("You did not accept"); },
                () => { Debug.LogError("You accepted"); }
            );
    }


}
