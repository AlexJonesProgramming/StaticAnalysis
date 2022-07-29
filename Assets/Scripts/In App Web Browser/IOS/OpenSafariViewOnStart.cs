using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSafariViewOnStart : MonoBehaviour
{
    void Start()
    {
        SafariView.LaunchURL("https://google.com");
    }
}
