using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    Dictionary<string, UnityEvent> eventMap = new Dictionary<string, UnityEvent>();

    private void Awake() {
        instance = this;
    }

    public void AddEvent(string eventName){
        if(eventMap.ContainsKey(eventName)) return;

        eventMap[eventName] = new UnityEvent();
    }

    public void AddListener(string eventName, UnityAction func){
        if(!eventMap.ContainsKey(eventName)) eventMap[eventName] = new UnityEvent();
        eventMap[eventName].AddListener(func);
    }

    public void RemoveListener(string eventName, UnityAction func){
        eventMap[eventName].RemoveListener(func);
    }

    public void Invoke(string eventName){
        if(!eventMap.ContainsKey(eventName)) eventMap[eventName] = new UnityEvent();
        eventMap[eventName].Invoke();
    }
}