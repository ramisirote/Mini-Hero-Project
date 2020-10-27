using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    public static bool GameIsPaused = false;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject powersMenusUI;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    private GameControler _gameController;

    private GameObject activeMenu = null;

    // private GameControler _gameController;
    private PlayerManager _playerManager;
    
    private float _timeScale = 1f;

    private bool _gameSavedAlready = false;

    private void Start() {
        _playerManager = GameObject.FindWithTag("Player")?.GetComponent<PlayerManager>();
        _gameController = GameObject.FindWithTag("GameController")?.GetComponent<GameControler>();
        
        saveButton.onClick.AddListener(SaveGame);
        loadButton.onClick.AddListener(LoadGame);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!GameIsPaused) {
                Pause();
            }
            else {
                Resume();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (!GameIsPaused) {
                Pause();
                PowersMenu();
            }
            else if(activeMenu != powersMenusUI){
                PowersMenu();
            }
            else {
                Resume();
            }
        }
    }

    void Pause() {
        GameIsPaused = true;
        
        pauseMenuUI.SetActive(true);
        activeMenu = pauseMenuUI;
        
        _timeScale = Time.timeScale;
        _playerManager.SetPause(true);
        Time.timeScale = 0f;
        
        _gameSavedAlready = false;
        if(saveButton) saveButton.gameObject.SetActive(true);
    }

    public void Resume() {
        
        GameIsPaused = false;
        activeMenu.SetActive(false);
        _playerManager.SetPause(false);
        Time.timeScale = _timeScale;
    }

    public void SaveGame() {
        if(_gameSavedAlready || !_gameController) return;
        
        _gameController.SaveGame();

        if(saveButton) saveButton.gameObject.SetActive(false);

        _gameSavedAlready = true;
    }
    
    public void LoadGame() {
        if(!_gameController) return;
        
        _gameController.LoadGame();
        
        Resume();
    }

    public void PowersMenu() {
        activeMenu.SetActive(false);
        activeMenu = powersMenusUI;
        activeMenu.SetActive(true);
    }
    
}
