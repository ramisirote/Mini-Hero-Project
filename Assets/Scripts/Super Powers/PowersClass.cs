using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterPowers", menuName = "CharacterPowers/PowerClass", order = 1)]
[System.Serializable]
public class PowersClass : ScriptableObject
{
    
    [SerializeField] private string powerName;
    [SerializeField] private Sprite powerIcon;
    [SerializeField] private PowerAbility[] powerPrefabs;
    public bool isSinglePower;
    
    // public Color color1;
    // public Color color2;
    // public Color color3;


    public PowerAbility GetPowerAt(int i) {
        if (powerPrefabs.Length == 0) return null;
        if (i >= powerPrefabs.Length || i < 0) return powerPrefabs[0];
        
        powerPrefabs[i].SetUpIcon();
        
        return powerPrefabs[i];
    }

    public PowerAbility GetPowerByName(PowerAbility.AbilityNames nameOfPower) {
        var ab = Array.Find(powerPrefabs, ability => ability.name == nameOfPower);
        // ab.SetColors(this);
        ab.SetUpIcon();
        return ab;
    }

    public PowerAbility GetPowerDefault() {
        return powerPrefabs.Length == 0 ? null : powerPrefabs[0];
    }

    public bool HasPowerObject(GameObject powerObject) {
        return Array.Exists(powerPrefabs, ability => ability.superPowerObg.name == powerObject.name);
    }

    public PowerAbility[] GetAbilites() {
        return powerPrefabs;
    }

    public string GetName() {
        return powerName;
    }
    
    public Sprite GetIcon() {
        return powerIcon;
    }
}

[System.Serializable]
public class PowerAbility
{
    public enum AbilityNames
    {
        FlameThrower, Blast, Flying, Teleportation, SuperSpeed, Strength, Invisibility, EnergyBurst, EnergyCharge,
        EnergyBeam
    }
    
    public AbilityNames name;
    public GameObject superPowerObg;
    public Color color1;
    public Color color2;
    public Color color3;
    // public bool isMainPower;
    public bool[] unlockes = new bool[4];
    public Sprite icon;

    public void SetUpIcon() {
        icon = superPowerObg.GetComponent<Ability>().GetIconImage();
    }
}
