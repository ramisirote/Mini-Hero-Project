using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PowerMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] abilityIcons;
    [SerializeField] private GameObject secondaryAbilityIcons;
    [SerializeField] private GameObject[] unlockButtons;

    [SerializeField] private Text unlockPointsText;

    private Image[] powerImages;

    // [SerializeField] private CharacterPowers characterPower;

    private PowersInstance _powersInstance;

    private CharacterStatsData _statsData;
    
    private PowerAbility[] _powerAbilitiesMain;
    private PowerAbility _secondaryPowerAbility;
    
    void Awake() {
        var playerObj = GameObject.FindWithTag("Player");
        _powersInstance = playerObj.GetComponent<PowersInstance>();
        _statsData = playerObj.GetComponent<CharacterStats>().GetCharacterStats();

        powerImages = new Image[abilityIcons.Length];
        for (int i = 0; i < abilityIcons.Length; i++) {
            powerImages[i] = abilityIcons[i].GetComponentsInChildren<Image>()[1];
        }
    }


    private void SetUp() {

        unlockPointsText.text = _statsData.UnlockPoints.ToString();
        
        _powerAbilitiesMain = _powersInstance.powers.mainPowers.GetAbilites();
        _secondaryPowerAbility = _powersInstance.powers.secondaryAbility;
        
        for(int i=0; i<abilityIcons.Length-1 && i<_powerAbilitiesMain.Length ;i++) {
            SetUpAbility(i, _powerAbilitiesMain[i]);
        }
        
        
        SetUpAbility(4, _secondaryPowerAbility);
    }

    private void SetUpAbility(int i, PowerAbility ability) {
        powerImages[i].enabled = true;
        powerImages[i].sprite = ability.icon;
        if (!_powersInstance.IsAbilityUnlocked(ability)) {
            powerImages[i].color = new Color(1,1,1,0.5f);
            unlockButtons[i].SetActive(true);
        }
        else {
            powerImages[i].color = Color.white;
            unlockButtons[i].SetActive(false);
        }
    }


    public void UnlockPowerMain(int powerNum) {
        if (!(_statsData.UnlockPoints > 0)) return;
        
        _statsData.UnlockPoints--;
        if (powerNum < 4) {
            _powersInstance.AddPowerAbility(_powerAbilitiesMain[powerNum].name, true);
        }
        else {
            _powersInstance.AddPowerAbility(_secondaryPowerAbility.name, false);
        }
        SetUp();
    }

    private void OnEnable() {
        SetUp();
    }
}
