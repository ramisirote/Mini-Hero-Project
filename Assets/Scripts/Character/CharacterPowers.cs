using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "CharacterPowers", menuName = "CharacterPowers/CharacterPowers", order = 1)]
public class CharacterPowers : ScriptableObject
{
    public SuperPowerLibrary powersLibrary;
    public PowersClass mainPowers;
    public PowersClass secondaryPowers;

    public List<PowerAbility> superPowers;

    public int active1 = -1;
    public int active2 = -1;

    public bool hasPowers = false;

    public PowerAbility secondaryAbility;
    
    [SerializeField] private Color[] mainColors = new Color[3];
    [SerializeField] private Color[] secColors = new Color[3];

    
    // Need to remove for power system update.
    public void SelectPower(GameObject newPower, int position) {
        if (position >= superPowers.Count) return;
        superPowers[position] = new PowerAbility{superPowerObg = newPower};
        hasPowers = true;
        
        FirstSetUpPower(position);
    }

    public void SetPowerClass(PowersClass powersClass, bool asMain) {
        if (asMain) {
            mainPowers = powersClass;
        }
        else {
            secondaryPowers = powersClass;
        }
    }

    public PowerAbility AddPowerFromClass(PowerAbility.AbilityNames powerName, bool fromMain) {
        var powersClass = fromMain ? mainPowers : secondaryPowers;
        
        var ability = powersClass.GetPowerByName(powerName);
        if (ability == null) return null;
        if (!fromMain) secondaryAbility = ability;
        
        SetAbilityColors(ability, fromMain);
        
        superPowers.Add(ability);
        
        FirstSetUpPower(superPowers.Count-1);

        return ability;
    }
    
    public PowerAbility AddPowerFromClass(int powerIndex, bool fromMain) {
        var powersClass = fromMain ? mainPowers : secondaryPowers;
        
        var ability = powersClass.GetPowerAt(powerIndex);
        if (ability == null) return null;

        if (!fromMain) secondaryAbility = ability;
        
        SetAbilityColors(ability, fromMain);
        
        superPowers.Add(ability);
        
        FirstSetUpPower(superPowers.Count-1);

        return ability;
    }

    public void SetPowerAsActive(int positionInList, int whichActive) {
        if(positionInList >= superPowers.Count) return;

        switch (whichActive) {
            case 1:
                active1 = positionInList;
                break;
            case 2: 
                active2 = positionInList;
                break;
        }
    }


    public void Init() {
        if (superPowers.Count > 1) {
            if (active1 == -1) {
                active1 = active2 == 0 ? 1 : 0;
            }

            if (active2 == -1) {
                active2 = active1 == 0 ? 1 : 0;
            }
        }
        else if(superPowers.Count == 1) {
            if (active1 == -1) active1 = 0;
        }
        
    }


    private void FirstSetUpPower(int pos) {
        if (active1 == -1) {
            active1 = pos;
        }
        else if(active2 == -1) {
            active2 = pos;
        }
    }

    private void SetAbilityColors(PowerAbility ability, bool isMain) {
        var colorArray = isMain ? mainColors : secColors;

        ability.color1 = colorArray[0];
        ability.color2 = colorArray[1];
        ability.color3 = colorArray[2];

    }


    public PowerAbility GetActive(int which) {
        return which == 1 ? superPowers[active1] : superPowers[active2];
    }

    public bool IsAbilityUnlocked(PowerAbility ability) {
        return superPowers.Exists(power => power.name == ability.name);
    }


    public void Clear() {
        superPowers.Clear();
        active1 = -1;
        active2 = -1;
    }


    public void SetStartingAbilities(int mainAbilityIndex, int secondaryAbilityIndex) {
        Clear();
        AddPowerFromClass(mainAbilityIndex, true);
        AddPowerFromClass(secondaryAbilityIndex, false);
    }

    public void SetSubColor(Color c,bool isMain, int sub) {
        if (isMain) {
            mainColors[sub] = c;
            foreach (var powerAbility in superPowers) {
                if (powerAbility.name != secondaryAbility.name) {
                    switch (sub) {
                        case 0: powerAbility.color1 = c; break;
                        case 1: powerAbility.color2 = c; break;
                        case 2: powerAbility.color3 = c; break;
                    }
                }
            }
        }
        else {
            secColors[sub] = c;
            switch (sub) {
                case 0: secondaryAbility.color1 = c; break;
                case 1: secondaryAbility.color2 = c; break;
                case 2: secondaryAbility.color3 = c; break;
            }
        }
    }


    public SerializedPowers Serialize() {
        return new SerializedPowers(this);
    }

    public void DeSerializedPowers(SerializedPowers ser) {
        mainPowers = powersLibrary.GetPowerClass(ser.mainPowerName);
        secondaryPowers = powersLibrary.GetPowerClass(ser.secPowerName);

        mainColors = Utils.FloatArrayToColorArray(ser.mainColors);
        secColors = Utils.FloatArrayToColorArray(ser.secondColors);
        
        superPowers.Clear();

        secondaryAbility = secondaryPowers.GetPowerByName((PowerAbility.AbilityNames) ser.secAbilityNameEnum);

        for (int i = 0; i < ser.abilityNamesEnum.Length; i++) {
            if (ser.abilityNamesEnum[i] != ser.secAbilityNameEnum) {
                superPowers.Add(mainPowers.GetPowerByName((PowerAbility.AbilityNames)ser.abilityNamesEnum[i]));
            }
            else {
                superPowers.Add(secondaryAbility);
            }

            superPowers[i].unlockes = ser.abilityUpgrades[i];
        }

        foreach (var ability in superPowers) {
            SetAbilityColors(ability, ability.name!=secondaryAbility.name);
        }

        active1 = ser.active1;
        active2 = ser.active2;
    }

    [System.Serializable]
    public class SerializedPowers
    {
        public string mainPowerName;
        public string secPowerName;

        public int[] abilityNamesEnum;
        public bool[][] abilityUpgrades;
        
        public int secAbilityNameEnum;

        public float[] mainColors;
        public float[] secondColors;

        public int active1;
        public int active2;

        public SerializedPowers(CharacterPowers other) {
            mainPowerName = other.mainPowers.GetName();
            secPowerName = other.secondaryPowers.GetName();
            
            abilityNamesEnum = new int[other.superPowers.Count];
            abilityUpgrades = new bool[other.superPowers.Count][];
            for (int i = 0; i < other.superPowers.Count; i++) {
                abilityNamesEnum[i] = (int)other.superPowers[i].name;
                abilityUpgrades[i] = other.superPowers[i].unlockes;
            }

            secAbilityNameEnum = (int)other.secondaryAbility.name;

            mainColors = Utils.ColorsArrayToFloatArray(other.mainColors);
            secondColors = Utils.ColorsArrayToFloatArray(other.secColors);

            active1 = other.active1;
            active2 = other.active2;
        }
    }
}
