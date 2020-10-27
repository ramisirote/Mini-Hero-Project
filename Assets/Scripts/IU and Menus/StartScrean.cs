using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScrean : MonoBehaviour
{
    [SerializeField] private GameObject loadButton;

    [SerializeField] private TextMeshProUGUI startButtonText;
    // Start is called before the first frame update
    void Start()
    {
        string path = Application.persistentDataPath + "/SavedGame.sav";
        Debug.Log(path);
        if (!File.Exists(path)) {
            loadButton.SetActive(false);
        }
        else {
            startButtonText.text = "NEW GAME";
        }
    }

    public void StartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
