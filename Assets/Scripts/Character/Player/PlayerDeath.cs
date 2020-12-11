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
    
    private PlayerManager _manager;
    private TakeDamage _takeDamage;
    private CharacterController2D _controller;
    private Animator _animator;

    private int _checkPointValue;
    
    private float restartTime;

    private void Awake() {
        _manager = gameObject.GetComponent<PlayerManager>();
        _takeDamage = gameObject.GetComponent<TakeDamage>();
        _controller = gameObject.GetComponent<CharacterController2D>();
        _animator = gameObject.GetComponent<Animator>();
        _takeDamage.OnDeathEvent += PlayerDead;

    }

    public void PlayerDead(object obj, IManager e) {
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
        if (_animator) _animator.SetBool(AnimRefarences.Dead, false);
        
        // activate controller
        if (_controller) _controller.enabled = true;

        if (_takeDamage) _takeDamage.enabled = true;
        
        // activate manager
        if (_manager) {
            _manager.enabled = true;
            _manager.EnableManager();
        }
        
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
