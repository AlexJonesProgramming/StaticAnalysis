using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class allowes you to fully close an android application (it will no longer show up in the previous apps menu)
/// </summary>
public class FullCloseAndroid : MonoBehaviour
{
    public void close()
    {
        //This is the class the holds the function we want to call
        AndroidJavaClass ChromeCustomTabInstance = new AndroidJavaClass("com.thermofisher.chromecustomtabsibrary.ChromeCustomTab");

        //These two lines get the Unity app activity (android activity)
        AndroidJavaClass UnityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject UnityPlayerActivity = UnityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        //Call the function in chrome custom tabs to close the application fully
        ChromeCustomTabInstance.CallStatic("FullCloseApplication", UnityPlayerActivity);
    }


    /* This function should be a part of the "ChromeCustomTab" class in our android pluging currently named "Classes.Jar"
    
     public static  void FullCloseApplication(Activity activity)
    {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            activity.finishAndRemoveTask();
        }
    }
     
     
     */

}
