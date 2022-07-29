using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices;


public class LinkWizard : MonoBehaviour
{
    #region Open Normal URLS

    public void OpenURLinternal(string url)
    {
#if UNITY_ANDROID

        ChromeCustomTabs.OpenChromeCustomTab(url);

#elif UNITY_IOS

        SafariView.LaunchURL(url);

#endif
    }

    public void OpenURLExternal(string url)
    {
        Application.OpenURL(url);
    }

    #endregion



    #region Opening links from JSON files on servers

    public void OpenURLFromJSONExternal(string urlOfJSON)
    {
        StartCoroutine(OpenURLFromJSONExternalEnum(urlOfJSON));
    }

    public void OpenURLFromJSONInternal(string urlOfJSON)
    {
        StartCoroutine(OpenURLFromJSONInternalEnum(urlOfJSON));
    }

    IEnumerator OpenURLFromJSONExternalEnum(string urlOfJSON)
    {
        UnityWebRequest www = new UnityWebRequest(urlOfJSON);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            LinkJSON jsonURL = JsonUtility.FromJson<LinkJSON>(www.downloadHandler.text);
            OpenURLExternal(jsonURL.url);
        }
    }

    IEnumerator OpenURLFromJSONInternalEnum(string urlOfJSON)
    {
        UnityWebRequest www = new UnityWebRequest(urlOfJSON);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {

            LinkJSON jsonURL = JsonUtility.FromJson<LinkJSON>(www.downloadHandler.text);
            OpenURLinternal(jsonURL.url);
        }
    }

    [System.Serializable]
    public struct LinkJSON
    {
        public string url;
    }

    #endregion



    #region Open Map link

    /// <summary>
    /// This currently is only a proof of concept
    /// </summary>
    public void OpenMap()
    {
#if UNITY_ANDROID
        //This is the class the holds the function we want to call
        AndroidJavaClass ChromeCustomTabInstance = new AndroidJavaClass("com.thermofisher.chromecustomtabsibrary.ChromeCustomTab");

        //These two lines get the Unity app activity (android activity)
        AndroidJavaClass UnityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject UnityPlayerActivity = UnityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        //Call the function in chrome custom tabs to open the map at this latitude and logitude.
        ChromeCustomTabInstance.CallStatic("OpenMapAtLocation", UnityPlayerActivity, 48.858314f, 2.294282f);
#else
        Debug.LogError("This function only works on android");
#endif
    }

    #endregion



    #region Open Deep Links


#if UNITY_IOS
    [DllImport("__Internal")]

    extern static void OpenDeepLink(string url);
#endif

    /// <summary>
    /// Open a deep link.
    /// </summary>
    /// <param name="url"></param>
    public static void openDeepLink(string url)
    {
#if UNITY_IOS
        OpenDeepLink(url);
#else
        Application.OpenURL(url);
#endif
    }

    #endregion
}
