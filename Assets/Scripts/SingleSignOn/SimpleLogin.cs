using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImaginationOverflow.UniversalDeepLinking;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Net.Http;
using System.Text;
using System;
//using Newtonsoft.Json;
using TMPro;
using System.Security.Cryptography;
using System.IO;

public class SimpleLogin : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// The endpoint for the log in
    /// </summary>
    private const string authorizationEndpoint = "https://aat0161.my.idaptive.app/OAuth2/Authorize/fselifestylesdev";
    /// <summary>
    /// The endpoint for exchanging the refresh token
    /// </summary>
    private const string refreshEndpoint = "https://github.com/login/oauth/use_refresh";
    /// <summary>
    /// The end point where the access token can be requested
    /// </summary>
    private const string tokenEndpoint = "https://thermofisher-dev.my.idaptive.app/OAuth2/Token/fselifestylesdev";
    /// <summary>
    /// The end point where user information can be requested
    /// </summary>
    private const string userInfoEndpoint = "https://thermofisher-dev.my.idaptive.app/OAuth2/UserInfo/fselifestylesdev";
    /// <summary>
    /// The end point where user is logged out
    /// </summary>
    private const string endSessionEndpoint = "https://thermofisher-dev.my.idaptive.app/OAuth2/EndSession/fselifestylesdev";

    //Information about our application needed to log in
    private string[] Scopes = new string[] { "openid", "profile", "email" };
    private string ClientId = "73d0bde3-850d-47da-8a3b-1ea3f1bab74c";
    [Tooltip("The encrypted ClientSecret")]
    public string ClientSecret = "";
    private string RedirectURI = "fselifestyles://dev";

    //Information gathered throughout the process of logging the user in
    private string authCode;
    private string id_token;
    private string access_token;
    private string refresh_token;
    private string codeVerifier;
    private string codeChalange;
    private string expires_in;
    private string auth_time;

    #endregion


    #region MonoBeahivour callbacks


    public void Awake()
    {
        //Awake is called before the application paused event happens
        //Init checks if the user is logged in the first time application paused is invoked.
        TFEvents.InitialStart.AddListener(Init);
    }


    #endregion


    /// <summary>
    /// I DONT KNOW WHERE TO CATAGORIZE THIS
    /// </summary>
    public void Init()
    {
        Debug.LogError("Init STARTED");
        //Check if the user creds are still valid.
        //This will load the creds from disk.
        ValidateUserLogIn();
        //Remove the calback to the Initial Start event, we only need to worry about this when we start the app from a cold start
        TFEvents.InitialStart.RemoveListener(Init);
    }


    #region UI Button functions


    public void OpenLoginButton()
    {
        OpenLoginPage(Scopes);
    }


    public void GetUserInfoButton()
    {
        StartCoroutine(GetUserInfo());
    }


    public void RefreshButton()
    {
        RefreshAccessToken();
    }


    #endregion


    #region Authentication


    /// <summary>
    /// Opens the iDaptive login page in the system's default Web browser
    /// </summary>
    /// <param name="scopes">The OpenID Connect scopes that the user must agree to</param>
    /// <param name="redirectUri">The URI to which the browser should redirect after the successful login</param>
    public void OpenLoginPage(string[] scopes)
    {
        //Generate the values needed to use the PKCE floow
        codeVerifier = PKCEHelper.GenerateNonce();
        codeChalange = PKCEHelper.GenerateCodeChallenge(codeVerifier);

        //Build a URL string that holds the scopes we need
        string uriScopes = WordArrayToSpaceEscapedString(scopes);

        //build the URI to open the auth portal in a browser.
        string uri = authorizationEndpoint + "?";
        uri += $"response_type=code&";
        uri += $"code_challenge={codeChalange}&";
        uri += $"code_challenge_method=S256&";
        uri += $"client_id={ClientId}&";
        uri += $"redirect_uri={RedirectURI}&";
        uri += $"scope={uriScopes}";

        TFEvents.DeepLinkRecieved.AddListener(DeepLinkRecieved);
        Application.OpenURL(uri);
       

        /* What the URI we need to open should look like.
        https://YOUR_DOMAIN/authorize?
        response_type = code &
        code_challenge = CODE_CHALLENGE &
        code_challenge_method = S256 &
        client_id = YOUR_CLIENT_ID &
        redirect_uri = YOUR_CALLBACK_URL &
        scope = SCOPE
        */
    }


    /// <summary>
    /// Waits for the login page to return a auth_code.
    /// Then parses the auth_code and passes it on to GetID_TokenFromAuth_Code();
    /// </summary>
    /// <param name="link"></param>
    private void DeepLinkRecieved(LinkActivation link)
    {
        //We need to check that the deep link was from the right place before we unsubscribe from the event
        //If the deep link has a code it in, it's probably the auth code
        if (link.QueryString.ContainsKey("code"))
        {
            TFEvents.DeepLinkRecieved.RemoveListener(DeepLinkRecieved);
            authCode = link.QueryString["code"];
            Debug.LogError("DeepLinkRecieved: Got an auth code!");
            Task.Run(() => GetID_TokenFromAuth_Code());
        }
        else
        {
            //Show the Deep Link anyways
            Debug.LogError("DeepLinkRecieved: Error: link contents= " + link.Uri);
        }
    }
    

    /// <summary>
    /// Takes the auth_code and requests an id_token from the token endpoint.
    /// If it is able to get an id_token it passes it off to be decoded.
    /// </summary>
    private async void GetID_TokenFromAuth_Code()
    {
        /* These are the feilds we need to send to the token endpoint.
        grant_type = authorization_code
        client_id=YOUR_CLIENT_ID
        code_verifier=YOUR_GENERATED_CODE_VERIFIER
        code=YOUR_AUTHORIZATION_CODE
        redirect_uri=https://YOUR_APP/callback"
        */

        Debug.LogError("GetID_TokenFromAuth_Code STARTED");
        Debug.LogError($"GetID_TokenFromAuth_Code: auth_code={authCode}");

        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(tokenEndpoint);
            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("redirect_uri", RedirectURI),
                new KeyValuePair<string, string>("code", authCode),
                new KeyValuePair<string, string>("code_verifier", codeVerifier),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            });
            Debug.LogError("In Try post, redirect URI is: " + RedirectURI);
            var result = await client.PostAsync("", content);

            if (result.IsSuccessStatusCode)
            {
                Debug.LogError("Successful response!");

                //Read the response JSON and parse it into a dictionary
                string resultContent = await result.Content.ReadAsStringAsync();
                Dictionary<string, string> values = JsonUtility.FromJson<Dictionary<string, string>>(resultContent);//JsonConvert.DeserializeObject<Dictionary<string, string>>(resultContent);

                //Check for and assign valuse from the response JSON.
                if (values.ContainsKey("id_token"))
                    id_token = values["id_token"];
                else
                    Debug.LogError("There was a response from the Token Endpoint but it did not contain a ID Token.");

                if (values.ContainsKey("access_token"))
                    access_token = values["access_token"];
                else
                    Debug.LogError("There was a response from the Token Endpoint but it did not contain a Access Token.");

                if (values.ContainsKey("refresh_token"))
                    refresh_token = values["refresh_token"];
                else
                    Debug.LogError("There was a response from the Token Endpoint but it did not contain a Refresh Token.");
                if(values.ContainsKey("expires_in"))
                    expires_in = values["expires_in"];
                else
                    Debug.LogError("There was a response from the Token Endpoint but it did not contain a 'expires in' time.");
                


                //Print out all of the keys sent back to us.
                foreach (KeyValuePair<string, string> pair in values)
                {
                    Debug.Log("GetID_TokenFromAuth_Code: "+ pair.Key + "=" + pair.Value);
                }

                //Try and get the user information.
                //StartCoroutine(GetUserInfoAsync());

                DecodeID_Token();
            }
            else 
            {
                string decoded = await result.Content.ReadAsStringAsync();
                Debug.LogError($"GetID_TokenFromAuth_Code: {result.StatusCode.ToString()}: {decoded}");
            }

        }
    }


    private async Task<bool> RefreshAccessToken()
    {
        /* These are the parameters we should send to refresh our access token.
        grant_type=refresh_token
        client_id=<My client ID>
        client_secret=<Mu client secret>
        refresh_token=<My refresh token>
        */

        Debug.LogError("RefreshAccessToken: STARTED");

        if (string.IsNullOrEmpty(refresh_token))
        {
            TFEvents.OpenConfirmDialogue.Invoke("No Refresh Token", "There was no refresh token.\nPlease login again.");
            Debug.LogError("Cannot refresh access token, no refresh token existed.");
            return false;
        }

        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(tokenEndpoint);

            //build the form data to send to the token enpoint.
            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", DecryptUserInfo(ClientSecret)),
                new KeyValuePair<string, string>("refresh_token", refresh_token)
            });
            //If you need to print this form to check that the data is correct use the line below.
            //string contentToString = await content.ReadAsStringAsync();

            //send the form to the token enpoind an wait for it to respond
            var result = await client.PostAsync("", content);

            if (result.IsSuccessStatusCode)
            {
                Debug.LogError("RefreshAccessToken: Successful refresh response!");

                //Read the response JSON and parse it into a dictionary
                string resultContent = await result.Content.ReadAsStringAsync();
                Dictionary<string, string> values = JsonUtility.FromJson<Dictionary<string, string>>(resultContent);//JsonConvert.DeserializeObject<Dictionary<string, string>>(resultContent);

                //Check for and assign valuse from the response JSON.
                if (values.ContainsKey("id_token"))
                    id_token = values["id_token"];
                else
                    Debug.LogError("There was a response from the Token Endpoint but it did not contain a ID Token.");

                if (values.ContainsKey("access_token"))
                    access_token = values["access_token"];
                else
                    Debug.LogError("There was a response from the Token Endpoint but it did not contain a Access Token.");

                if (values.ContainsKey("refresh_token"))
                    refresh_token = values["refresh_token"];
                else
                    Debug.LogError("There was a response from the Token Endpoint but it did not contain a Refresh Token.");

                DecodeID_Token();
                return true;
            }
            else
            {
                string decoded = await result.Content.ReadAsStringAsync();
                Debug.LogError($"RefreshAccessToken: {result.StatusCode.ToString()}: {result.Content}: {decoded}");

                //We couldn't refresh the access_token so the user needs to log in again.
                PromtUserToLogin();
            }
        }

        return false;
    }


    /// <summary>
    /// Uses the authorization token to retrieve user information from the user_info endpoint.
    /// </summary>
    IEnumerator GetUserInfo()
    {
        Debug.LogError("GetUserInfo: STARTED");

        //Check that we have an acess token
        if (!string.IsNullOrEmpty(access_token))
        {
            //set up a web request to the user info enpoint
            UnityWebRequest uwr = UnityWebRequest.Get(userInfoEndpoint);
            uwr.SetRequestHeader("Authorization", "Bearer " + access_token);
            yield return uwr.SendWebRequest();

            //Wait for the results of the reques
            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("GetUserInfo: Error While Sending: " + uwr.error);
                TFEvents.OpenConfirmDialogue.Invoke("Error While Sending", uwr.error);
            }
            else
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);
                //Trigger an event that contains the user information as a JSON
                TFEvents.LoadedUserInfo.Invoke(uwr.downloadHandler.text);
            }
        }
        else
        {
            TFEvents.OpenConfirmDialogue.Invoke("No Access Token", "There was no access token.\nPlease login again.");
            Debug.LogError("Cannot get user info, no access token existed");
            RefreshAccessToken();
        }
    }


    /// <summary>
    /// Opens the end_session_endpoint
    /// </summary>
    public void LogUserOut()
    {
        ClearUserInfoFromDisk();

        authCode = "";
        id_token = "";
        access_token = "";
        refresh_token = "";
        codeVerifier = "";
        codeChalange = "";
        expires_in = "";
        auth_time = "";

        Application.OpenURL(endSessionEndpoint);

        LogInScreenManager.Instance.EnableLoginScreen();
    }


    /// <summary>
    /// Takes the id_token returned from the token endpoint, which is a JSON web token, and de-encodes it to grab the user information.
    /// </summary>
    private void DecodeID_Token()
    {
        Debug.LogError("DecodeID_Token STARTED");
        LogInScreenManager.Instance.SetLoadingBar(0.5f, "Attempting to log in...");

        if (string.IsNullOrEmpty(id_token))
        {
            Debug.LogError("Cannot get user information from ID token: There is no ID Token.");
            return;
        }

        //Decode and parse the ID Token
        Dictionary<string, string> values = new Dictionary<string, string>();
        var parts = id_token.Split('.');
        if (parts.Length > 2)
        {
            var decode = parts[1];
            var padLength = 4 - decode.Length % 4;
            if (padLength < 4)
            {
                decode += new string('=', padLength);
            }

            //convert the JWT to plain text so that we can send it through the event.
            var bytes = System.Convert.FromBase64String(decode);
            var userInfo = System.Text.ASCIIEncoding.ASCII.GetString(bytes);

            //Trigger an event the has the user info
            TFEvents.LoadedUserInfo.Invoke(userInfo);

            //We need the info here to find the auth time.
            values = JsonUtility.FromJson<Dictionary<string, string>>(userInfo);//JsonConvert.DeserializeObject<Dictionary<string, string>>(userInfo);
            foreach (KeyValuePair<string, string> pair in values)
            {
                Debug.Log("DecodeID_Token: " + pair.Key + "=" + pair.Value);
            }
        }

        LogInScreenManager.Instance.SetLoadingBar(0.75f, "Attempting to log in...");

        if (values.ContainsKey("auth_time"))
            auth_time = values["auth_time"];
        else
            Debug.LogError("There was a response from the Token Endpoint but it did not contain a auth time.");

        LogInScreenManager.Instance.SetLoadingBar(0.9f, "Attempting to log in...");
        SaveUserInfoToDisk();
        LogInScreenManager.Instance.DisableLoginScreen();
    }


    /// <summary>
    /// Checks if we have a valid login.
    /// If we do not it will prompt the user to log in.
    /// </summary>
    private void ValidateUserLogIn()
    {
        Debug.LogError("ValidatUserLogIn STARTED");
        LogInScreenManager.Instance.SetLoadingBar(0.1f, "checking for login session...");
        //Check if we need to load the user info from the disk
        if (string.IsNullOrEmpty(access_token))
        {
            Debug.LogError("ValidateUserLogIn: No access token found, attempting to load info from disk.");
            LoadUserInfoFromDisk();
        }

        //Double check that we have user information after loading from disk
        if (string.IsNullOrEmpty(access_token))
        {
            //We have no user information so we need them to login
            Debug.LogError("ValidateUserLogIn: No access token, please log in.");
            PromtUserToLogin();
            return;
        }

        LogInScreenManager.Instance.SetLoadingBar(0.2f, "checking for login session...");

        //Check if the access_token has expired
        if (AccessTokenValid())
        {
            DecodeID_Token();
            return; // It hasn't! The user is still logged in we don't need to do anything
        }

        LogInScreenManager.Instance.SetLoadingBar(0.3f, "checking for login session...");

        //Check if we can renew it
        if (RefreshTokenValid())
        {
            //We can! so we renew it and the user is still logged in
            Task.Run(() => RefreshAccessToken());
            return;
        }

        //Prompt the user to log in.
        Debug.LogError("ValidateUserLogIn: could not refresh access token, please log in.");
        PromtUserToLogin();

        //We return false because the user is not logged in.
        return;
    }


    /// <summary>
    /// Checks if the access token is still in it's allotted time
    /// </summary>
    /// <returns>True if the access token is still useable, false if it has expired.</returns>
    private bool AccessTokenValid()
    {
        //Make sure we have all the information to log in
        if (string.IsNullOrEmpty(auth_time) || string.IsNullOrEmpty(refresh_token) || string.IsNullOrEmpty(expires_in))
            return false;

        int issueTime = Convert.ToInt32(auth_time); // auth_time is a string
        int expires = Convert.ToInt32(expires_in); // expires_in is a string

        //Give us 10 seconds to use the token
        if (CurrentEpochTime() < (issueTime + expires - 10))
            return true;

        Debug.LogError($"AccessTokenValid: access token expired {CurrentEpochTime() - (issueTime + expires)} seconds ago.");
        return false;
    }


    /// <summary>
    /// Checks if the refresh token is still in it's allotted time
    /// </summary>
    /// <returns>True if the refresh token is still useable, false if it has expired.</returns>
    private bool RefreshTokenValid()
    {
        //Make sure we have all the information to log in
        if (string.IsNullOrEmpty(auth_time) || string.IsNullOrEmpty(refresh_token))
        {
            Debug.LogError($"RefreshTokenValid: the auth_time or refresh_token was not set.\nauth_time: {auth_time}\nrefresh_token{refresh_token}");
            return false;
        }

        int issueTime = Convert.ToInt32(auth_time); //Auth time is a string
        int expires = 432000; //this is hard coded to 5 days

        //Give us 10 seconds to use the token
        if (CurrentEpochTime() < (issueTime + expires - 10))
            return true;

        Debug.LogError($"RefreshTokenValid: refresh token expired {CurrentEpochTime() - (issueTime + expires)} seconds ago.");
        return false;
    }


    #endregion


    #region Local storage

    private void SaveUserInfoToDisk()
    {
        /* Information that we want to save.
        id_token : Has all the information about the user
        access_token : allows access to APIs
        refresh_token : Allows us to get a new access_token
        expires_in : This is how long the access_token is valid from the issue time
        auth_time : Epoch Unix Timestamp from when the token was authorized
        */

        Debug.LogError("Saving user info to disk...");

        if(!string.IsNullOrEmpty(id_token))
        {
            string encoded = EncryptUserInfo(id_token);
            PlayerPrefs.SetString("id_token", encoded);
        }
        if (!string.IsNullOrEmpty(access_token))
        {
            string encoded = EncryptUserInfo(access_token);
            PlayerPrefs.SetString("access_token", encoded);
        }
        if (!string.IsNullOrEmpty(refresh_token))
        {
            string encoded = EncryptUserInfo(refresh_token);
            PlayerPrefs.SetString("refresh_token", encoded);
        }
        if (!string.IsNullOrEmpty(expires_in))
        {
            string encoded = EncryptUserInfo(expires_in);
            PlayerPrefs.SetString("expires_in", encoded);
        }
        if (!string.IsNullOrEmpty(auth_time))
        {
            string encoded = EncryptUserInfo(auth_time);
            PlayerPrefs.SetString("auth_time", encoded);
        }
    }


    private void LoadUserInfoFromDisk()
    {
        Debug.LogError("Loading user info from disk...");

        if(PlayerPrefs.HasKey("id_token"))
        {
            string decoded = PlayerPrefs.GetString("id_token");
            id_token = DecryptUserInfo(decoded);
            Debug.LogError("id_token: " + id_token);
        }
        if (PlayerPrefs.HasKey("access_token"))
        {
            string decoded = PlayerPrefs.GetString("access_token");
            access_token = DecryptUserInfo(decoded);
            Debug.LogError("access_token: " + access_token);
        }
        if (PlayerPrefs.HasKey("refresh_token"))
        {
            string decoded = PlayerPrefs.GetString("refresh_token");
            refresh_token = DecryptUserInfo(decoded);
            Debug.LogError("refresh_token: " + refresh_token);
        }
        if (PlayerPrefs.HasKey("expires_in"))
        {
            string decoded = PlayerPrefs.GetString("expires_in");
            expires_in = DecryptUserInfo(decoded);
            Debug.LogError("expires_in: " + expires_in);
        }
        if (PlayerPrefs.HasKey("auth_time"))
        {
            string decoded = PlayerPrefs.GetString("auth_time");
            auth_time = DecryptUserInfo(decoded);
            Debug.LogError("auth_time: " + auth_time);
        }
    }


    private void ClearUserInfoFromDisk()
    {
        PlayerPrefs.DeleteKey("id_token");
        PlayerPrefs.DeleteKey("access_token");
        PlayerPrefs.DeleteKey("refresh_token");
        PlayerPrefs.DeleteKey("expires_in");
        PlayerPrefs.DeleteKey("auth_time");
    }


    private string EncryptUserInfo(string textToEncrypt)
    {
        try
        {
            string ToReturn = "";
            string publickey = "12FE9EAC275F35B8";
            string secretkey = "207B7F6513D3E5F0";
            byte[] secretkeyByte = { };
            secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
            byte[] publickeybyte = { };
            publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
            MemoryStream ms = null;
            CryptoStream cs = null;
            byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                ms = new MemoryStream();
                cs = new CryptoStream(ms, aes.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                cs.FlushFinalBlock();
                ToReturn = Convert.ToBase64String(ms.ToArray());
            }
            return ToReturn;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex.InnerException);
        }
    }


    private string DecryptUserInfo(string textToDecrypt)
    {
        try
        {
            string ToReturn = "";
            string publickey = "12FE9EAC275F35B8";
            string secretkey = "207B7F6513D3E5F0";
            byte[] privatekeyByte = { };
            privatekeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
            byte[] publickeybyte = { };
            publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
            MemoryStream ms = null;
            CryptoStream cs = null;
            byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
            inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                ms = new MemoryStream();
                cs = new CryptoStream(ms, aes.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                ToReturn = encoding.GetString(ms.ToArray());
            }
            return ToReturn;
        }
        catch (Exception ae)
        {
            throw new Exception(ae.Message, ae.InnerException);
        }
    }


    #endregion


    #region Utility

    /// <summary>
    /// Returns the current Epoch Time
    /// </summary>
    /// <returns></returns>
    private int CurrentEpochTime()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;

        return currentEpochTime;
    }

    /// <summary>
    /// Takes an array of strings and returns a single string 
    /// that will be accepted as a list in a URL parameter
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
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


    /// <summary>
    /// Pareses a URL and returns a string, string, dictionary
    /// containing the paramaters in the URL and their values
    /// </summary>
    /// <param name="query">The URL you want to parse</param>
    /// <returns></returns>
    private Dictionary<string, string> ParseURLQueries(string query)
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

    /// <summary>
    /// Opens a prompt telling the user they need to log in to use the application
    /// </summary>
    private void PromtUserToLogin()
    {
        //This enables the login screen.
        LogInScreenManager.Instance.EnableLoginScreen();
        LogInScreenManager.Instance.DisableLoadingBar();
    }

    #endregion
}


/*===================== SIMPLE EXPLINATION ====================

Open log in page opens the log in page passing information SSO needs to validate the user this incluedes:
	response_type
        code_challenge
        code_challenge_method
        client_id
        redirect_uri
        scope
It then subscribes DeepLinkRecieved to the deep link event.


Deep link recived should get information back from the SSO webpage, passed in as part of the deep link URL.
this information contains an auth code. If there is an auth code Deep link recived unsubscribes from the deep link event
and passes the auth token to GetID_TokenFromAuth_Code.


GetID_TokenFromAuth_Code send the auth code over a back channel to the SSO servers along with our code_challange_verifier
and gets back an in_token, acess_token, refresh_token, and expires_in.

The ID token holds all the user information we would want. Name Email, Phone number. 
This is a JSON web token so we need to decode it and pull all the information out.

DecodeID_Token decodes the id_token and saves the user info to the disk so that we can log in again when we reopen the app.


There are a few other functions of note in this script.

RefreshAccessToken sends our refresh token to the SSO server and then gets back the same information as GetID_TokenFromAuth_Code.
We then decode the id_token again.

Log user out, does what it says

ValidateUser is called the first time the user starts the app (cold start) it loads the information from the disk and checks
that the tokens are all still valid (within their expiry time).

GetUserInfo uses the access token to get the user information from the SSO sever, this is different from decoding the access token.

 */