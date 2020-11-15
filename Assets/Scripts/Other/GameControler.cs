using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameControler : MonoBehaviour
{
    [SerializeField] private CharacterAppearance playerAppearance;
    [SerializeField] private CharacterPowers playerPowers;
    
    private static GameControler _gameControler = null;
    
    private GameObject _player;
    private GameObject _uiOverlay;

    private CharacterStatsData _characterStatsData = null;

    private int _level;

    private static bool _setUp = false;


    private int _screenshotFileSaveNum = 0;
    private float _nextTimeCanScreenshot;

    private void Awake() {
        if (_gameControler == null) {
            DontDestroyOnLoad(gameObject);
            _gameControler = this;
            
            OnLevelLoad();
        }
        else {
            _characterStatsData = _gameControler._characterStatsData;
            _gameControler.OnLevelLoad();
            Destroy(this);
        }
    }

    private void OnLevelLoad() {
        FindPlayerAndOverlay();
        if(!_player) return;
        
        if (!_setUp) {
            _characterStatsData = _player.GetComponent<CharacterStats>().GetCharacterStats();
        }
        else {
            _player.GetComponent<CharacterStats>().SetCharacterStats(_characterStatsData);
        }
        if(_uiOverlay) _uiOverlay.GetComponentInChildren<StatsDisplay>().SetStats(_characterStatsData);

        SetAllEnemyDeathTriggers();

        _setUp = true;
    }

    private void FindPlayerAndOverlay() {
        _player = GameObject.FindWithTag("Player");
        _uiOverlay = GameObject.FindWithTag("UI");
    }

    public CharacterStatsData GetCharacterStats() {
        return _characterStatsData;
    }

    public void SetCharatcerStats(CharacterStatsData characterStatsDataOther) {
        _characterStatsData = characterStatsDataOther;

        if (_uiOverlay) {
            StatsDisplay statsDisplay = _uiOverlay.GetComponentInChildren<StatsDisplay>();
        
            if (statsDisplay) {
                statsDisplay.SetStats(_characterStatsData);
            }
        }

        _setUp = true;
    }

    private void SetStatsPlayerAndDisplay(CharacterStatsData statsData) {
        SetCharatcerStats(statsData);
        SetPlayerStats(statsData);
    }

    private void SetPlayerStats(CharacterStatsData statsData) {
        if (!_player) {
            _player = GameObject.FindWithTag("Player");
        }
        if(_player) _player.GetComponent<CharacterStats>()?.SetCharacterStats(statsData);
    }

    private void SetAllEnemyDeathTriggers() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies) {
            enemy.GetComponent<TakeDamage>().OnDeathEvent += EnemyDeathAddXpToPlayer;
        }
    }

    // Update is called once per frame
    void Update() {
        
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        if (Time.time > _nextTimeCanScreenshot && Input.GetKeyDown(KeyCode.LeftShift)) {
            ScreenCapture.CaptureScreenshot("Screenshots\\Screenshot_"+_screenshotFileSaveNum+".png");
            _screenshotFileSaveNum++;
            _nextTimeCanScreenshot = Time.time + 1f;
        }

        if (Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.V)
            || Input.GetKeyDown(KeyCode.V) && Input.GetKey(KeyCode.L)) {
            _characterStatsData.AddXp(100);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            if (Time.timeScale == 0) {
                Time.timeScale = 1;
            }
            else {
                Time.timeScale = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.T)
            || Input.GetKeyDown(KeyCode.T) && Input.GetKey(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        
    }

    public void EnemyDeathAddXpToPlayer(object sender, EventArgs e) {
        if (sender == null) return;
        int xp = 0;
        AIBase enemyAI = ((GameObject) sender).GetComponent<AIBase>();
        if (enemyAI) {
            xp = enemyAI.GetXp();
        }
        
        _characterStatsData?.AddXp(xp);
    }

    public void SaveGame() {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/SavedGame.sav";
        FileStream stream = new FileStream(path, FileMode.Create);
        
        GameSave save = new GameSave {
            statsFields = _characterStatsData.GetFieldsArray(), 
            levelBuildIndex = SceneManager.GetActiveScene().buildIndex,
            characterAppearance = playerAppearance.GetSerialize(),
            serializedPowers = playerPowers.Serialize()
        };
        
        formatter.Serialize(stream, save);
        
        stream.Close();
    }
    
    public void LoadGame() {

        string path = Application.persistentDataPath + "/SavedGame.sav";

        if (File.Exists(path)) {
            
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
        
            var save = (GameSave)formatter.Deserialize(stream);
            
            SetStatsPlayerAndDisplay(new CharacterStatsData(save.statsFields));

            playerAppearance.CopyFromSerialized(save.characterAppearance);
            
            stream.Close();

            SceneManager.LoadScene(save.levelBuildIndex);
            
            playerPowers.DeSerializedPowers(save.serializedPowers);
        }
        else {
            Debug.Log("Save File Does Not Exist");
        }
        
    }

    [System.Serializable]
    public struct GameSave
    {
        public float[] statsFields;
        public int levelBuildIndex;
        public CharacterAppearance.SerializedCharacterAppearance characterAppearance;
        public CharacterPowers.SerializedPowers serializedPowers;
    }
}
