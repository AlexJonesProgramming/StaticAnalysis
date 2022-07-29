using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class WebViewNavigationArrows : MonoBehaviour
{

    public Button back, forward;
    public SampleWebView sampleWebView;
    WebViewObject webView;

    bool canGoBack = false;
    bool canGoForward = false;

    UnityEvent<bool> goBackUpdated;
    UnityEvent<bool> goForwardUpdated;

    private void OnEnable()
    {
        goBackUpdated = new UnityEvent<bool>();
        goForwardUpdated = new UnityEvent<bool>();

        goBackUpdated.AddListener(UpdateBackArrow);
        goForwardUpdated.AddListener(UpdateForwardArrow);

        back.onClick.AddListener(GoBack);
        forward.onClick.AddListener(GoForward);

    }

    // Update is called once per frame
    void Update()
    {

        if (webView == null)
        {
            Debug.LogError("There was no webView, trying again.");
            webView = sampleWebView.GetWebViewObject();

            if (webView == null)
                return;
            else
                Debug.LogError("webView found!");
        }

        if (webView.CanGoBack() != canGoBack)
        {
            canGoBack = !canGoBack;
            goBackUpdated?.Invoke(webView.CanGoBack());
        }

        if(webView.CanGoForward() != canGoForward)
        {
            canGoForward = !canGoForward;
            goForwardUpdated?.Invoke(webView.CanGoForward());
        }
    }

    void UpdateBackArrow(bool value)
    {
        back.interactable = value;
    }

    void UpdateForwardArrow(bool value)
    {
        forward.interactable = value;
    }

    private void OnDisable()
    {
        goBackUpdated.RemoveAllListeners();
        goForwardUpdated.RemoveAllListeners();
        goBackUpdated = null;
        goForwardUpdated = null;

        back.onClick.RemoveListener(GoBack);
        forward.onClick.RemoveListener(GoForward);
    }



    void GoBack()
    {
        webView?.GoBack();
    }

    void GoForward()
    {
        webView?.GoForward();
    }

}
