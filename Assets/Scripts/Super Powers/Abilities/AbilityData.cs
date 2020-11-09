using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is all the static parts of a ability. The name, icon description, game object ece...
 */
[CreateAssetMenu(fileName = "NewAbility", menuName = "CharacterPowers/AbilityData", order = 1)]
[System.Serializable]
public class AbilityData : ScriptableObject
{
    public enum AbilityEnum
    {
        FlameThrower, Blast, Flying, Teleportation, SuperSpeed, Strength, Invisibility, EnergyBurst, EnergyCharge,
        EnergyBeam, SpeedDash, StrengthSmash, TornadoRun
    }

    public string abilityName;
    public AbilityEnum abilityEnum;
    public Sprite icon;
    public GameObject abilityGameObject;
    public string powerClassName;
    public string description;
    public Upgrade[] upgrades = new Upgrade[UpgradeMask.NUMBER_OF_UPGRADES];

    [System.Serializable]
    public class Upgrade
    {
        [SerializeField] private string upgradeName;
        [SerializeField] private string upgradeDescription;
    }
}
