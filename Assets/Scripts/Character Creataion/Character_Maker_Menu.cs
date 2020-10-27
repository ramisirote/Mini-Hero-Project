using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Character_Maker_Menu : MonoBehaviour
{
    private GameObject _uiOverlay;

    public void Awake() {
        _uiOverlay = GameObject.FindWithTag("UI");
        if (_uiOverlay) {
            _uiOverlay.SetActive(false);
        }
    }

    public void PlayGame() {
        if (_uiOverlay) {
            _uiOverlay.SetActive(false);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
