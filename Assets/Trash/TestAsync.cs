using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Proof that you can invoke an event in an async funnction
/// </summary>
public class TestAsync : MonoBehaviour
{


    private UnityEvent myEvent;



    private void Awake()
    {
        myEvent = new UnityEvent();
        myEvent.AddListener(EventFunc);
    }

    private void OnDestroy()
    {
        myEvent.RemoveListener(EventFunc);
    }


    // Start is called before the first frame update
    void Start()
    {
        Task t = Task.Run(AsyncFunc);
        Debug.LogError("Start");
    }

    private void Update()
    {
        Debug.LogError("Update");
    }



    private async Task AsyncFunc()
    {
        Debug.LogError("Async start");
        await Task.Delay(3000);
        Debug.LogError("after delay");
        myEvent.Invoke();
    }


    private void EventFunc()
    {
        Debug.LogError("Event Triggered from asyncFunction");
    }


}
