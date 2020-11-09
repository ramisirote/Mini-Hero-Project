using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    [FormerlySerializedAs("characterStats")] [SerializeField] private CharacterStatsData characterStatsData;
    private int _level;
    private bool _disableEnergyRecharge;

    private void Start() {
        if (characterStatsData == null) {
            characterStatsData = new CharacterStatsData();
        }
    }

    public void DisableEnergyRecharge(bool disable) {
        _disableEnergyRecharge = disable;
    }

    public void SetCharacterStats(CharacterStatsData characterStatsDataOther) {
        characterStatsData = characterStatsDataOther;
    }

    private void Update() {
        characterStatsData.RegenerateHealth(Time.deltaTime);
        if (!_disableEnergyRecharge) {
            characterStatsData.RegenerateEnergy(Time.deltaTime);
        }
    }

    public bool IsDead() {
        return characterStatsData.Health <= 0;
    }

    public bool UseEnergy(float amountNeeded) {
        if (characterStatsData.Energy >= amountNeeded) {
            characterStatsData.ChangeEnergyBy(-1*amountNeeded);
            return true;
        }

        return false;
    }

    public void ResetFromStats(CharacterStatsData statsDataToResetTo) {
        characterStatsData.CopyStats(statsDataToResetTo);
    }

    public void ResetStats() {
        characterStatsData.SetHealthToMax();
        characterStatsData.SetEnergyToMax();
    }

    public CharacterStatsData GetCharacterStats() {
        return characterStatsData ?? (characterStatsData = new CharacterStatsData());
    }
}
