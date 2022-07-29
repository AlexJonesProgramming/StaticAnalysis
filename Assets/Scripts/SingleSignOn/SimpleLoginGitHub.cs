using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImaginationOverflow.UniversalDeepLinking;
using System.Threading.Tasks;
using UnityEngine.Networking;


public class SimpleLoginGitGub : MonoBehaviour
{
    /// <summary>
    /// The endpoint for the log in
    /// </summary>
    private const string authorizationEndpoint = "https://github.com/login/oauth/authorize";
    /// <summary>
    /// The endpoint for exchanging the refresh token
    /// </summary>
    private const string refreshEndpoint = "https://github.com/login/oauth/use_refresh";
    /// <summary>
    /// The end point where the access token can be requested
    /// </summary>
    private const string tokenEndpoint = "https://github.com/login/oauth/access_token";
    /// <summary>
    /// The end point where user information can be requested
    /// </summary>
    private const string userInfoEndpoint = "https://api.github.com/user";

    public string[] Scopes { get; set; } = new string[] { "openid", "profile", "email" };
    public string ClientId = "170bcb6f9cf0e8e711b3";
    public string ClientSecret = "c18c185bd35217f395bafc4d218523a51595eb85";



    private string authCode;
    private string accessToken;



    public void TestButton()
    {
        OpenLoginPage(Scopes, "fselifestyles://dev");
    }


    /// <summary>
    /// Opens the iDaptive login page in the system's default Web browser
    /// </summary>
    /// <param name="scopes">The OpenID Connect scopes that the user must agree to</param>
    /// <param name="redirectUri">The URI to which the browser should redirect after the successful login</param>
    public void OpenLoginPage(string[] scopes, string redirectUri)
    {

        string responseType = "code"; // This is what we want back from the log in system:  code or token;
        string uriScopes = WordArrayToSpaceEscapedString(scopes);
        string uri = authorizationEndpoint + $"?client_id={ClientId}&redirect_uri={redirectUri}" + $"&response_type={responseType}&scope={uriScopes}";
        Application.OpenURL(uri);
        TFEvents.DeepLinkRecieved.AddListener(DeepLinkRecieved);
    }

    private void DeepLinkRecieved(LinkActivation link)
    {
        //We need to check that the deep link was from the right place before we unsubscribe from the event
        //If the deep link has a code it in, it's probably the auth code
        if (link.QueryString.ContainsKey("code"))
        {
            TFEvents.DeepLinkRecieved.RemoveListener(DeepLinkRecieved);
            authCode = link.QueryString["code"];

            TFEvents.OpenAcceptDialogue.Invoke
                (
                    "Got an Auth Code!",
                    $"Auth Code: {authCode}",
                    () => { Debug.LogError("You didn't want to get the acess token"); },
                    () => { StartCoroutine( GetAccessTokenFromCodeAsync(authCode, "fselifestyles://") ); }
                );
        }
        else
        {
            //So the Deep Link anyways
            TFEvents.OpenConfirmDialogue.Invoke("Deep Link Contents", link.Uri);
        }
    }

    IEnumerator GetAccessTokenFromCodeAsync(string code, string redirectUri)
    {
        Debug.LogError("GetAccessTokenFromCodeAsync STARTED");

        WWWForm form = new WWWForm();
        form.AddField("client_id", ClientId);
        form.AddField("redirect_uri", redirectUri);
        form.AddField("client_secret", ClientSecret);
        form.AddField("code", code);
        form.AddField("grant_type", "authorization_code");


        using (UnityWebRequest www = UnityWebRequest.Post(tokenEndpoint, form))
        {
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                TFEvents.OpenConfirmDialogue.Invoke("Response not Successful", $"Form Data: {form.data}");

            }
            else
            {
                string responseText = www.downloadHandler.text;
                Dictionary<string, string> queries = ParseURLQueries(responseText);

                if(queries.ContainsKey("access_token"))
                {
                    accessToken = queries["access_token"];

                    TFEvents.OpenAcceptDialogue.Invoke
                        (
                            "You got an Access Token!",
                            $"{accessToken}",
                            () => { Debug.LogError("You didn't wanna use the access token."); },
                            () => { StartCoroutine(GetUserInfoAsync()); }
                        );
                }
                else
                {
                    Debug.LogError("There was no acess token");
                }
            }
        }
    }

    IEnumerator GetUserInfoAsync()
    {
        //Dictionary<string, string> headers = new Dictionary<string, string>();
        //headers.Add("Authorization", $"token {accessToken}");
        //WebResponse<string> webResponse = await RestConnector.GetAsync(userInfoEndpoint + "?access_token=" + accessToken, headers);

        if(string.IsNullOrEmpty(accessToken))
        {
            Debug.LogError("Cannot get user data : No access token");
            yield break;
        }

        using (UnityWebRequest webRequest = UnityWebRequest.Get(userInfoEndpoint))
        {
            webRequest.SetRequestHeader("Authorization", $"token {accessToken}");
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = userInfoEndpoint.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    TFEvents.OpenConfirmDialogue.Invoke
                        (
                            "User Information",
                            $"{webRequest.downloadHandler.text}"
                        );
                    break;
            }
        }
    }

    #region Utility

    private string WordArrayToSpaceEscapedString(string[] array)
    {
        string concatArray = "";
        for (int i = 0; i < array.Length; i++)
        {
            concatArray += array[i];
            if (i < array.Length - 1)
            {
                concatArray += "%20";
            }
        }
        return concatArray;
    }

    public Dictionary<string, string> ParseURLQueries(string query)
    {
        Dictionary<string, string> outDict = new Dictionary<string, string>();
        
        int index = query.IndexOf('?');
        if (index > -1)
        {
            if (query.Length >= index + 1)
                query = query.Substring(index + 1);
        }
        var pairs = query.Split('&');
        foreach (var pair in pairs)
        {
            int index2 = pair.IndexOf('=');
            if (index2 > 0)
            {
                var key = pair.Substring(0, index2);
                var value = pair.Substring(index2 + 1);

                var origKey = key;
                int val = 2;
                while (outDict.ContainsKey(key))
                {
                    key = origKey + val++;
                }

                outDict.Add(key, value);
            }
        }

        return outDict;
    }

    #endregion
}
