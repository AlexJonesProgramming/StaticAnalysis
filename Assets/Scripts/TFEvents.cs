using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ImaginationOverflow.UniversalDeepLinking;

public static class TFEvents
{
    #region UI EVENTS 
    
    private static UnityEvent<string, string> _OpenConfirmDialogue;
    public static UnityEvent<string, string> OpenConfirmDialogue
    {
        private set => _OpenConfirmDialogue = value;
        get
        {
            if (_OpenConfirmDialogue == null)
                _OpenConfirmDialogue = new UnityEvent<string, string>();

            return _OpenConfirmDialogue;
        }
    }

    private static UnityEvent<string, string, UnityAction, UnityAction> _OpenAcceptDialogue;
    /// <summary>
    /// Param 1 : Header
    /// Param 2 : Body
    /// Param 3 : Decline Function
    /// Param 4 : Accept Function
    /// </summary>
    public static UnityEvent<string, string, UnityAction, UnityAction> OpenAcceptDialogue
    {
        private set => _OpenAcceptDialogue = value;
        get
        {
            if (_OpenAcceptDialogue == null)
                _OpenAcceptDialogue = new UnityEvent<string, string, UnityAction, UnityAction>();

            return _OpenAcceptDialogue;
        }
    }

    #endregion


    #region DEEP LINKING EVENTS

    private static UnityEvent<LinkActivation> _DeepLinkRecieved;
    public static UnityEvent<LinkActivation> DeepLinkRecieved
    {
        private set => _DeepLinkRecieved = value;
        get
        {
            if (_DeepLinkRecieved == null)
                _DeepLinkRecieved = new UnityEvent<LinkActivation>();

            return _DeepLinkRecieved;
        }
    }

    #endregion


    #region APPLICATION STATUS EVENTS

    private static UnityEvent _InitialStart;
    public static UnityEvent InitialStart
    {
        private set => _InitialStart = value;
        get
        {
            if (_InitialStart == null)
                _InitialStart = new UnityEvent();

            return _InitialStart;
        }
    }


    private static UnityEvent _ApplicationPaused;
    public static UnityEvent ApplicationPaused
    {
        private set => _ApplicationPaused = value;
        get
        {
            if (_ApplicationPaused == null)
                _ApplicationPaused = new UnityEvent();

            return _ApplicationPaused;
        }
    }


    private static UnityEvent _ApplicationUnpaused;
    public static UnityEvent ApplicationUnpaused
    {
        private set => _ApplicationUnpaused = value;
        get
        {
            if (_ApplicationUnpaused == null)
                _ApplicationUnpaused = new UnityEvent();

            return _ApplicationUnpaused;
        }
    }


    private static UnityEvent _ApplicationLostFocus;
    public static UnityEvent ApplicationLostFocus
    {
        private set => _ApplicationLostFocus = value;
        get
        {
            if (_ApplicationLostFocus == null)
                _ApplicationLostFocus = new UnityEvent();

            return _ApplicationLostFocus;
        }
    }


    private static UnityEvent _ApplicationGainedFocus;
    public static UnityEvent ApplicationGainedFocus
    {
        private set => _ApplicationGainedFocus = value;
        get
        {
            if (_ApplicationGainedFocus == null)
                _ApplicationGainedFocus = new UnityEvent();

            return _ApplicationGainedFocus;
        }
    }


    #endregion


    #region AUTHENTICATION EVENTS

    /// <summary>
    /// Used by Simple User Loging to send user information to UserInfoUI
    /// </summary>
    private static UnityEvent<string> _LoadedUserInfo;
    public static UnityEvent<string> LoadedUserInfo
    {
        private set => _LoadedUserInfo = value;
        get
        {
            if (_LoadedUserInfo == null)
                _LoadedUserInfo = new UnityEvent<string>();

            return _LoadedUserInfo;
        }
    }

    #endregion

}
