using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class allows us to open a chrome tab in our application.
/// </summary>
public class ChromeCustomTabs : MonoBehaviour
{

#if UNITY_ANDROID

    public static void OpenChromeCustomTab(string url)
    {
        //Make and instance of the chrome custom tabs class
        AndroidJavaClass ChromeCustomTab = new AndroidJavaClass("com.thermofisher.chromecustomtabsibrary.ChromeCustomTab");
        //Find the current instance of the unity player
        AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //Get the android activity from the unity player
        AndroidJavaObject PlayerActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        //Call the function to open the browser
        ChromeCustomTab.CallStatic("OpenCustomTab", url, PlayerActivity, 1, 1, 1);
    }

#endif

    /*
    
    In Assets/Plugins/Android there should be a .JAR file named "Classes"
    This is a compiled java project that contains the code we are running above.

    */
}
