using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalDialogue : MonoBehaviour
{

    [Header("Confirmation Dialogue")]
    public GameObject confirmationDialogue;
    public TextMeshProUGUI confirmationTitle, confirmationDescription;

    [Header("Accept Dialogue")]
    public GameObject acceptDialogue;
    public TextMeshProUGUI acceptTitle, acceptDescription;
    public Button acceptAcceptButton, acceptDeclineButton;

    #region Event Subscriptions
    private void OnEnable()
    {
        TFEvents.OpenConfirmDialogue.AddListener(OpenConfirmation);
        TFEvents.OpenAcceptDialogue.AddListener(OpenAccept);
    }
    private void OnDisable()
    {
        TFEvents.OpenConfirmDialogue.RemoveListener(OpenConfirmation);
        TFEvents.OpenAcceptDialogue.RemoveListener(OpenAccept);
    }
    #endregion

    #region Event Callbacks


    /// <summary>
    /// Open a dialogue that is meant only to present information to the user.
    /// No action can be taken from this dialogue.
    /// </summary>
    /// <param name="title">The header of the dialogue</param>
    /// <param name="description">The body of the dialogue</param>
    public void OpenConfirmation(string title, string description)
    {
        confirmationTitle.text = title;
        confirmationDescription.text = description;
        confirmationDialogue.SetActive(true);
        //The confirm button should turn this back off.
    }

    public void OpenAccept(string title, string description, UnityAction declineAction, UnityAction acceptAction)
    {
        acceptTitle.text = title;
        acceptDescription.text = description;

        acceptDialogue.SetActive(true);

        acceptAcceptButton.onClick.RemoveAllListeners();
        acceptAcceptButton.onClick.AddListener(acceptAction);
        acceptAcceptButton.onClick.AddListener(() => {acceptDialogue.SetActive(false); });

        acceptDeclineButton.onClick.RemoveAllListeners();
        acceptDeclineButton.onClick.AddListener(declineAction);
        acceptDeclineButton.onClick.AddListener(() => { acceptDialogue.SetActive(false); });
    }

    #endregion
}
