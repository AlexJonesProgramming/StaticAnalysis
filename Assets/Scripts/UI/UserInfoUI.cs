using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;


/// <summary>
/// This class listenes for events about user info (Name email etc.)
/// and sets UI elements that use that info.
/// </summary>
public class UserInfoUI : MonoBehaviour
{
    // TMPro components to display user information.
    public TextMeshProUGUI firstNameUI, lastNameUI, emailUI;


    private void Awake()
    {
        TFEvents.LoadedUserInfo.AddListener(UpdateUserInfo);
    }

    /// <summary>
    /// Updates UI that user user information.
    /// </summary>
    /// <param name="info">A JSON that contains user information</param>
    public void UpdateUserInfo(string info)
    {
        // This doesn't see to cause in problems if the LoadedUserInfo event is triggered in an async function.

        Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(info);

        string firstName = "";
        string lastName = "";
        string email = "";

        if (values.ContainsKey("given_name"))
        {
            firstName = values["given_name"];
        }
        if (values.ContainsKey("family_name"))
        {
            lastName = values["family_name"];
        }
        if (values.ContainsKey("email"))
        {
            email = values["email"];
        }

        firstNameUI.text = "Welcome back " + firstName;
        lastNameUI.text = "Last Name : " + lastName;
        emailUI.text = "Email : " + email;
    }


    private void OnDestroy()
    {
        TFEvents.LoadedUserInfo.RemoveListener(UpdateUserInfo);
    }

}
