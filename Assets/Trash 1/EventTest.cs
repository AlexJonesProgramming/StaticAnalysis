using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTest : MonoBehaviour
{
    void Start()
    {
        Debug.LogError("Start");       
    }
    void Update()
    {
        Debug.LogError("Update");
    }

    private void LateUpdate()
    {
        Debug.LogError("LateUpdate");
    }

    private void OnApplicationPause(bool pause)
    {
        Debug.LogError("Pause");
    }

    private void Awake()
    {
        Debug.LogError("Awake");
        TFEvents.InitialStart.AddListener(Listener);
    }

    private void OnEnable()
    {
        Debug.LogError("OnEnable");
    }


    void Listener()
    {
        Debug.LogError("Triggered by initial start event!");
    }

}
