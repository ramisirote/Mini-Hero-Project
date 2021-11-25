using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PowerMenu : MonoBehaviour
{
    [SerializeField] private List<AbilityColumn> abilityColums;

    [SerializeField] private Text unlockPointsText;

    // [SerializeField] private CharacterPowers characterPower;

    private CharacterPowerManager _powersManager;

    private CharacterStatsData _statsData;
    
    void Awake() {
        _powersManager = GameControler.GetPowerManager();
        var playerObj = _powersManager.gameObject;
        _statsData = playerObj.GetComponent<CharacterStats>().GetCharacterStats();
        unlockPointsText.text = _statsData.UnlockPoints.ToString();

        SetUp();
        _statsData.OnUnlockPoitnsChange += OnUnlockPoitnsChange;
    }

    private void OnUnlockPoitnsChange(object sender, EventArgs e) {
        unlockPointsText.text = _statsData.UnlockPoints.ToString();
    }

    private void SetUp() {
        for (int i = 0; i < _powersManager.abilities.Count; i++) {
            abilityColums[i].SetData(_powersManager.abilities[i]);
        }
    }

    private void OnEnable() {
        SetUp();
    }
}
