using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadSceanPrefab : MonoBehaviour
{
    private static float _nextCanTransitionTime;
    
    [SerializeField] private GameObject currentScene;
    [SerializeField] private GameObject sceneToLoad;
    [SerializeField] private string transitionPointName;


    private void OnTriggerEnter2D(Collider2D other) {
        if( Time.time < _nextCanTransitionTime) return;
        
        if (other.CompareTag("Player")) {
            _nextCanTransitionTime = Time.time + 0.5f;
            GameControler.GetGameControler().LoadScenePrefab(currentScene, sceneToLoad, transitionPointName);
        }
    }
}
