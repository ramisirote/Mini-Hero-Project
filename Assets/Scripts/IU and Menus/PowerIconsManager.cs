using System;
using System.Collections;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class PowerIconsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] powerIcons;
    [SerializeField] private UI_Overlay _uiOverlay;
    
    private Image[] _powerIconsImage;
    private Animator[] _powerIconsAnimarots;

    private PlayerManager _playerManager;

    private Ability[] _superPowers;
    
    Dictionary<Ability, int> powerToIcon = new Dictionary<Ability, int>();
    
    private void Start() {
        
        _powerIconsImage = new Image[powerIcons.Length];
        _powerIconsAnimarots = new Animator[powerIcons.Length];
        for (int i = 0; i < powerIcons.Length; i++) {
            _powerIconsImage[i] = powerIcons[i].GetComponentsInChildren<Image>()[1];
            _powerIconsAnimarots[i] = powerIcons[i].GetComponent<Animator>();
        }
        
        
        _playerManager = GameObject.FindWithTag("Player")?.GetComponent<PlayerManager>();
        if (_playerManager == null) {
            return;
        }
        
        _superPowers = _playerManager.GetAbilities();

        for (int i = 0; i < powerIcons.Length; i++) {
            if (_superPowers[i] != null) {
                powerToIcon[_superPowers[i]] = i;

                _superPowers[i].AbilityOnEvent += OnAbilityOnEvent;
                _superPowers[i].AbilityOffEvent += OnAbilityOffEvent;

                _powerIconsImage[i].sprite = _superPowers[i].GetIconImage();
                _powerIconsImage[i].enabled = true;
                
            }
            
        }
        
        _playerManager.ActivePowerChange += PlayerManagerOnActivePowerChange;
    }

    private void PlayerManagerOnActivePowerChange(object sender, int i) {
        if(i==-1){
            ClearIcons();
            return;
        }
        
        // Remove the current active power
        if (_superPowers[i]) {
            powerToIcon.Remove(_superPowers[i]);
            _superPowers[i].AbilityOnEvent -= OnAbilityOnEvent;
            _superPowers[i].AbilityOffEvent -= OnAbilityOffEvent;
        }
        
        // Reget the active powers
        _superPowers[i] = _playerManager.GetAbilities()[i];
        if (!_superPowers[i]) return;
        
        // set up the active power
        powerToIcon[_superPowers[i]] = i;

        _superPowers[i].AbilityOnEvent += OnAbilityOnEvent;
        _superPowers[i].AbilityOffEvent += OnAbilityOffEvent;

        _powerIconsImage[i].sprite = _superPowers[i].GetIconImage();
        _powerIconsImage[i].enabled = true;
    }

    private void OnAbilityOnEvent(object sender, EventArgs e) {
        int i = powerToIcon[(Ability) sender];
        if(_powerIconsAnimarots[i]) _powerIconsAnimarots[i].SetBool("On", true);
    }
    
    private void OnAbilityOffEvent(object sender, EventArgs e) {
        int i = powerToIcon[(Ability) sender];
        if(_powerIconsAnimarots[i]) _powerIconsAnimarots[i].SetBool("On", false);
    }

    private void ClearIcons() {
        for (int i = 0; i < _superPowers.Length; i++) {
            if (_superPowers[i]) {
                powerToIcon.Remove(_superPowers[i]);
                _superPowers[i].AbilityOnEvent -= OnAbilityOnEvent;
                _superPowers[i].AbilityOffEvent -= OnAbilityOffEvent;
            }
            
            _powerIconsImage[i].sprite = null;
            _powerIconsImage[i].enabled = false;
        }
    }
}
