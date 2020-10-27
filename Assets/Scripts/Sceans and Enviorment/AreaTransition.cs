using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaTransition : MonoBehaviour
{
    [SerializeField] private SceneAsset sceneToLoad;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            if (sceneToLoad) {
                SceneManager.LoadSceneAsync(sceneToLoad.name);
            }
            else if(SceneManager.sceneCountInBuildSettings > SceneManager.GetActiveScene().buildIndex + 1 ){
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
