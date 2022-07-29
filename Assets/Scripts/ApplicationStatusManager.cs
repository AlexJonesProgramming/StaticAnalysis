using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class is used to fire events when the application status changes.
/// Events include: Application paused, started, lost focus, gained focus, first start.
/// </summary>
public class ApplicationStatusManager : MonoBehaviour
{
    public static ApplicationStatusManager Instance { get; private set; }

    private bool initialStart = false;


    private void Awake()
    {
        //Set up the static instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
            Destroy(this);
    }

    private void OnApplicationPause(bool pause)
    {
        //Double check that we are the instance, just to be sure.
        if (Instance != this)
            return;

        if (pause)
        {
            //The application was exited by the user
            TFEvents.ApplicationPaused.Invoke();
        }
        else
        {
            //The application was opened by the user (Also happens the first time the app was launched)
            if(initialStart == false)
            {
                //this is the first time the user has opened the application (cold start)
                initialStart = true;
                TFEvents.InitialStart.Invoke();
            }
            else 
            {
                //The user has left the application and returned.
                TFEvents.ApplicationUnpaused.Invoke();
            }
        }
    }


    /// <summary>
    /// Trigged when the application gains or looses focus
    /// </summary>
    /// <param name="focus">True if the application has gained focus, false if the application has lost focus</param>
    private void OnApplicationFocus(bool focus)
    {
        if(focus)
        {
            //The application gained focus
            TFEvents.ApplicationGainedFocus.Invoke();
        }
        else 
        {
            //The application lost focus
            TFEvents.ApplicationLostFocus.Invoke();
        }
    }
}
