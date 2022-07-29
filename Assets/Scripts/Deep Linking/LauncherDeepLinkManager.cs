using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImaginationOverflow.UniversalDeepLinking;
using JLi_Utility;
using TMPro;

public class LauncherDeepLinkManager : Singleton<LauncherDeepLinkManager>
{
    //public TextMeshProUGUI text;
    //private static readonly string FORMAT = "{0}://{1}{2}";
    //public string baseDomainURI = "digitalinnovations.thermofisher.com/digitalinnovations/Testing/";
    //public GameObject fadeBackground;
    public float waitDuration = 1f;

    private Coroutine loadingURI;
    private bool leftApp = false;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        DeepLinkManager.Instance.LinkActivated += Instance_LinkActivated;
    }

    private void OnDisable()
    {
        DeepLinkManager.Instance.LinkActivated -= Instance_LinkActivated;
    }

    /// <summary>
    /// Function to take do something with the URI like loading a specific scene
    /// </summary>
    /// <param name="link"></param>
    private void Instance_LinkActivated(LinkActivation link)
    {
        string s = "";

        foreach(KeyValuePair<string, string> pair in link.QueryString)
        {
            s += $"{pair.Key} : {pair.Value} \n";
        }

        Debug.LogError(link.Uri);
        TFEvents.DeepLinkRecieved.Invoke(link);
    }

    public void StartURILoad()
    {
        if(loadingURI == null)
        {
            loadingURI = StartCoroutine(LoadURICo());
        }
        else
        {
            Debugger.LogError("Attempted to load URI when already trying to load one!");
        }
    }

    /// <summary>
    /// This is used to see if the app is not installed.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadURICo()
    {
        
        //string appURI = string.Format(FORMAT, card.baseURI, "", "");
        //Debugger.Log($"Attempting to open installed app with uri: {appURI}");
        //Application.OpenURL(appURI);
        yield return new WaitForSeconds(waitDuration);
        if (!leftApp)
        {
            //string webURI = string.Format(FORMAT, card.webScheme, baseDomainURI, card.baseURI + ".html");
            //Debugger.Log($"Attempting to open website with uri: {webURI}");
            //Application.OpenURL(webURI);
            yield return new WaitForSeconds(waitDuration);
            if (!leftApp)
            {
                Debugger.LogError("Failed to load website");
            }
        }
        ExitLoadURI();
    }

    private void OnApplicationPause(bool pause)
    {
        if(loadingURI != null)
            leftApp = true;
    }

    private void ExitLoadURI()
    {
        leftApp = false;
        loadingURI = null;
    }
}
