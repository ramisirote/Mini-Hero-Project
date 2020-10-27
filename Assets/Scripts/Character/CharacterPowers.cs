using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


/*
 * This scriptable object contains the current powers of the players and the colors selected for those powers,
 * as well the the abilities the player has unlocked from those powers.
 * It also saves which of the abilities is currently selected in the two selected ability slots.
 *
 * Implements adding abilities and changing which ability is active.
 */
[CreateAssetMenu(fileName = "CharacterPowers", menuName = "CharacterPowers/CharacterPowers", order = 1)]
public class CharacterPowers : ScriptableObject
{
    public SuperPowerLibrary powersLibrary;
    public PowersClass mainPowers;
    public PowersClass secondaryPowers;

    [FormerlySerializedAs("superPowers")] public List<PowerAbility> abilities;

    public int active1 = -1;
    public int active2 = -1;

    [FormerlySerializedAs("hasPowers")] public bool hasAbilities = false;

    public PowerAbility secondaryAbility;
    
    [SerializeField] private Color[] mainColors = new Color[3];
    [SerializeField] private Color[] secColors = new Color[3];

    
    public void SelectAbility(GameObject abilityGameObject, int position) {
        if (position >= abilities.Count) return;
        abilities[position] = new PowerAbility{abilityGameObg = abilityGameObject};
        hasAbilities = true;
        
        FirstSetUpAbility(position);
    }

    public void SetPowerClass(PowersClass powersClass, bool asMain) {
        if (asMain) {
            mainPowers = powersClass;
        }
        else {
            secondaryPowers = powersClass;
        }
    }

    public PowerAbility AddAbilityFromClass(PowerAbility.AbilityNames abilityName, bool fromMain) {
        var powersClass = fromMain ? mainPowers : secondaryPowers;
        
        var ability = powersClass.GetPowerByName(abilityName);
        if (ability == null) return null;
        if (!fromMain) secondaryAbility = ability;
        
        SetAbilityColors(ability, fromMain);
        
        abilities.Add(ability);
        
        FirstSetUpAbility(abilities.Count-1);

        return ability;
    }
    
    public PowerAbility AddAbilityFromClass(int abilityIndex, bool fromMain) {
        var powersClass = fromMain ? mainPowers : secondaryPowers;
        
        var ability = powersClass.GetAbilityAt(abilityIndex);
        if (ability == null) return null;

        if (!fromMain) secondaryAbility = ability;
        
        SetAbilityColors(ability, fromMain);
        
        abilities.Add(ability);
        
        FirstSetUpAbility(abilities.Count-1);

        return ability;
    }

    public void SetAbilityAsActive(int positionInList, int whichActive) {
        if(positionInList >= abilities.Count) return;

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
        if (abilities.Count > 1) {
            if (active1 == -1) {
                active1 = active2 == 0 ? 1 : 0;
            }

            if (active2 == -1) {
                active2 = active1 == 0 ? 1 : 0;
            }
        }
        else if(abilities.Count == 1) {
            if (active1 == -1) active1 = 0;
        }
        
    }


    private void FirstSetUpAbility(int pos) {
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
        return which == 1 ? abilities[active1] : abilities[active2];
    }

    public bool IsAbilityUnlocked(PowerAbility ability) {
        return abilities.Exists(power => power.name == ability.name);
    }


    public void Clear() {
        abilities.Clear();
        active1 = -1;
        active2 = -1;
    }


    public void SetStartingAbilities(int mainAbilityIndex, int secondaryAbilityIndex) {
        Clear();
        AddAbilityFromClass(mainAbilityIndex, true);
        AddAbilityFromClass(secondaryAbilityIndex, false);
    }

    public void SetSubColor(Color c,bool isMain, int sub) {
        if (isMain) {
            mainColors[sub] = c;
            foreach (var powerAbility in abilities) {
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
        
        abilities.Clear();

        secondaryAbility = secondaryPowers.GetPowerByName((PowerAbility.AbilityNames) ser.secAbilityNameEnum);

        for (int i = 0; i < ser.abilityNamesEnum.Length; i++) {
            if (ser.abilityNamesEnum[i] != ser.secAbilityNameEnum) {
                abilities.Add(mainPowers.GetPowerByName((PowerAbility.AbilityNames)ser.abilityNamesEnum[i]));
            }
            else {
                abilities.Add(secondaryAbility);
            }

            abilities[i].unlockes = ser.abilityUpgrades[i];
        }

        foreach (var ability in abilities) {
            SetAbilityColors(ability, ability.name!=secondaryAbility.name);
        }

        active1 = ser.active1;
        active2 = ser.active2;
    }

    /*
     * This class is a serialized version of the base class for saving purposes.
     */
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
            
            abilityNamesEnum = new int[other.abilities.Count];
            abilityUpgrades = new bool[other.abilities.Count][];
            for (int i = 0; i < other.abilities.Count; i++) {
                abilityNamesEnum[i] = (int)other.abilities[i].name;
                abilityUpgrades[i] = other.abilities[i].unlockes;
            }

            secAbilityNameEnum = (int)other.secondaryAbility.name;

            mainColors = Utils.ColorsArrayToFloatArray(other.mainColors);
            secondColors = Utils.ColorsArrayToFloatArray(other.secColors);

            active1 = other.active1;
            active2 = other.active2;
        }
    }
}
