using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerDeath : MonoBehaviour
{
    private static GameObject _player;
    
    [FormerlySerializedAs("stats")] [SerializeField] private CharacterStats characterStats;
    [SerializeField] private float deathDownTime;
    private Transform _checkPoint = null;
    private CharacterStatsData _checkPointStatsData = null;

    private int _checkPointValue;
    
    private float restartTime;

    private void Awake() {
        // if (_player == null) {
        //     _player = gameObject;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else {
        //     Destroy(_player);
        // }
        GetComponent<TakeDamage>().OnDeathEvent += PlayerDead;

    }

    public void PlayerDead(object obj, EventArgs e) {
        StartCoroutine(_checkPoint ? RestartCheckpoint() : RestartLevel());
    }

    public void SetCheckPoint(Transform checkPointToSet, CharacterStatsData resetToStatsData ,int checkPointValue) {
        if (checkPointValue >= _checkPointValue) {
            _checkPoint = checkPointToSet;
            _checkPointStatsData = resetToStatsData;
        }
    }

    private void MakeNotDead() {
        // Set Animator to not dead
        transform.gameObject.GetComponent<Animator>()?.SetBool(AnimRefarences.Dead, false);
        
        // activate controller
        CharacterController2D controller2D = transform.gameObject.GetComponent<CharacterController2D>();
        if (controller2D) {
            controller2D.enabled = true;
        }

        GetComponent<TakeDamage>().enabled = true;
        
        // activate manager
        GetComponent<IManager>()?.EnableManager();
        
        // return to player layer
        gameObject.layer = LayerMask.NameToLayer("Player");

    }

    private IEnumerator RestartCheckpoint() {
        yield return new WaitForSeconds(deathDownTime);
        
        transform.position = _checkPoint.position;
        if (_checkPointStatsData != null) {
            characterStats.ResetFromStats(_checkPointStatsData);
        }
        
        MakeNotDead();
    }

    private IEnumerator RestartLevel() {
        yield return new WaitForSeconds(deathDownTime);
        
        characterStats.ResetStats();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
