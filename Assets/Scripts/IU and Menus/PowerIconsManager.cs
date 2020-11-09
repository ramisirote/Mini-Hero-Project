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
    [SerializeField] private Slider[] cooldownSliders;

    private Image[] _powerIconsImage;
    private Animator[] _powerIconsAnimators;

    private PlayerManager _playerManager;

    private Ability[] _managerAbilities;

    private readonly Dictionary<Ability, int> powerToIcon = new Dictionary<Ability, int>();
    
    private void Start() {
        
        _powerIconsImage = new Image[powerIcons.Length];
        _powerIconsAnimators = new Animator[powerIcons.Length];
        for (int i = 0; i < powerIcons.Length; i++) {
            _powerIconsImage[i] = powerIcons[i].GetComponentsInChildren<Image>()[1];
            _powerIconsAnimators[i] = powerIcons[i].GetComponent<Animator>();
        }
        
        
        _playerManager = GameObject.FindWithTag("Player")?.GetComponent<PlayerManager>();
        if (_playerManager == null) {
            return;
        }
        
        _managerAbilities = _playerManager.GetAbilities();

        for (int i = 0; i < powerIcons.Length; i++) {
            if (_managerAbilities[i] != null) {
                powerToIcon[_managerAbilities[i]] = i;

                _managerAbilities[i].AbilityOnEvent += OnAbilityOnEvent;
                _managerAbilities[i].AbilityOffEvent += OnAbilityOffEvent;

                _powerIconsImage[i].sprite = _managerAbilities[i].GetIconImage();
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
        if (_managerAbilities[i]) {
            powerToIcon.Remove(_managerAbilities[i]);
            _managerAbilities[i].AbilityOnEvent -= OnAbilityOnEvent;
            _managerAbilities[i].AbilityOffEvent -= OnAbilityOffEvent;
            cooldownSliders[i].value = 0f;
        }
        
        // Reget the active powers
        _managerAbilities[i] = _playerManager.GetAbilities()[i];
        if (!_managerAbilities[i]) return;
        
        // set up the active power
        powerToIcon[_managerAbilities[i]] = i;

        _managerAbilities[i].AbilityOnEvent += OnAbilityOnEvent;
        _managerAbilities[i].AbilityOffEvent += OnAbilityOffEvent;

        _powerIconsImage[i].sprite = _managerAbilities[i].GetIconImage();
        _powerIconsImage[i].enabled = true;
    }

    private void OnAbilityOnEvent(object sender, EventArgs e) {
        int i = powerToIcon[(Ability) sender];
        if(_powerIconsAnimators[i]) _powerIconsAnimators[i].SetBool("On", true);
    }
    
    private void OnAbilityOffEvent(object sender, float cooldown) {
        int i = powerToIcon[(Ability) sender];
        if(_powerIconsAnimators[i]) _powerIconsAnimators[i].SetBool("On", false);
        StartCoroutine(Cooldown(cooldownSliders[i], cooldown));
    }

    IEnumerator Cooldown(Slider slider, float cooldown) {
        if (cooldown <= 0) yield break;
        
        float cooldownDone = Time.time + cooldown;
        slider.value = 1;
        while (slider.value > 0f) {
            slider.value = Math.Max(0, (cooldownDone - Time.time)/cooldown);
            yield return true;
        }
    }

    private void ClearIcons() {
        for (int i = 0; i < _managerAbilities.Length; i++) {
            if (_managerAbilities[i]) {
                powerToIcon.Remove(_managerAbilities[i]);
                _managerAbilities[i].AbilityOnEvent -= OnAbilityOnEvent;
                _managerAbilities[i].AbilityOffEvent -= OnAbilityOffEvent;
            }
            
            _powerIconsImage[i].sprite = null;
            _powerIconsImage[i].enabled = false;
        }
    }
}
