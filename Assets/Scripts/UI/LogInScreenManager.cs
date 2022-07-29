using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogInScreenManager : MonoBehaviour
{
    public static LogInScreenManager Instance { get; private set; }


    [SerializeField]
    private GameObject logInCanvas, logInButton, loadingBar, loadingStatusMessage;
    [SerializeField]
    private Image loadingProgressImage;
    [SerializeField]
    private TextMeshProUGUI loadingStatusMessageText;

    private bool loadingBarActive = false;
    private bool loadingBarActiveLastFrame = false;


    private bool LoginScreenActive = true;
    private bool LoginScreenActiveLastFrame = true;


    private string loadingProgressText;
    private float loadingProgressPercentage;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }



    private void Update()
    {
        LoadingBarUpdate();
        LoginScreenUpdate();
    }



    private void LoadingBarUpdate()
    {
        if (loadingBarActive)
        {
            if (loadingBarActiveLastFrame == false)
            {
                loadingBarActive = true;
                loadingBar.SetActive(true);
                loadingStatusMessage.SetActive(true);
                loadingStatusMessageText.text = "Loading...";
                DisableLoginButtion();
            }

            if (!string.IsNullOrEmpty(loadingProgressText))
                loadingStatusMessageText.text = loadingProgressText;

            if (CanvasUpdateRegistry.IsRebuildingGraphics() || CanvasUpdateRegistry.IsRebuildingLayout())
                return; //The UI is rebuilding we don't wanna update


            loadingProgressImage.fillAmount = Mathf.Clamp01(loadingProgressPercentage);
        }
        else
        {
            if (loadingBarActiveLastFrame == true)
            {

                loadingBar.SetActive(false);
                loadingStatusMessage.SetActive(false);
                EnableLoginButton();
            }
        }

        //This needs set last
        loadingBarActiveLastFrame = loadingBarActive;
    }


    private void LoginScreenUpdate()
    {
        if (LoginScreenActive)
        {
            if (LoginScreenActiveLastFrame == false)
            {
                logInCanvas.SetActive(true);
            }
        }
        else
        {
            if (LoginScreenActiveLastFrame == true)
            {
                DisableLoadingBar();
                logInCanvas.SetActive(false);
            }
        }

        //This needs set last
        LoginScreenActiveLastFrame = LoginScreenActive;
    }


    public void SetLoadingBar(float completion, string status = "")
    {
        loadingProgressText = status;
        loadingProgressPercentage = completion;
        loadingBarActive = true;
    }

    public void DisableLoadingBar()
    {
        loadingBarActive = false;
    }

    private void EnableLoginButton()
    {
        logInButton.SetActive(true);
    }

    private void DisableLoginButtion()
    {
        logInButton.SetActive(false);
    }

    public void EnableLoginScreen()
    {
        LoginScreenActive = true;
    }

    public void DisableLoginScreen()
    {
        LoginScreenActive = false;
    }

}
