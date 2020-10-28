using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CharacterPowers", menuName = "CharacterPowers/PowerClass", order = 1)]
[System.Serializable]
public class PowersClass : ScriptableObject
{
    [SerializeField] private string powerName;
    [SerializeField] private Sprite powerIcon;
    [SerializeField] private AbilityData[] abilitiesInClass;
    public bool isSinglePower;


    public AbilityData GetAbilityAt(int i) {
        if (abilitiesInClass.Length == 0) return null;
        if (i >= abilitiesInClass.Length || i < 0) return abilitiesInClass[0];

        return abilitiesInClass[i];
    }

    public AbilityData GetPowerByEnum(AbilityData.AbilityEnum enumOfAbility) {
        var ab = Array.Find(abilitiesInClass, ability => ability.abilityEnum == enumOfAbility);
        return ab;
    }

    public AbilityData GetPowerDefault() {
        return abilitiesInClass.Length == 0 ? null : abilitiesInClass[0];
    }

    public bool HasPowerObject(GameObject abilityObject) {
        return Array.Exists(abilitiesInClass, ability => ability.abilityGameObject.name == abilityObject.name);
    }

    public AbilityData[] GetAbilities() {
        return abilitiesInClass;
    }

    public string GetName() {
        return powerName;
    }
    
    public Sprite GetIcon() {
        return powerIcon;
    }
}

// [System.Serializable]
// public class PowerAbility
// {
//     public enum AbilityNames
//     {
//         FlameThrower, Blast, Flying, Teleportation, SuperSpeed, Strength, Invisibility, EnergyBurst, EnergyCharge,
//         EnergyBeam
//     }
//     
//     public AbilityNames name;
//     [FormerlySerializedAs("superPowerObg")] public GameObject abilityGameObg;
//     public Color color1;
//     public Color color2;
//     public Color color3;
//     // public bool isMainPower;
//     public bool[] unlockes = new bool[4];
//     public Sprite icon;
//
//     public void SetUpIcon() {
//         icon = abilityGameObg.GetComponent<Ability>().GetIconImage();
//     }
// }
