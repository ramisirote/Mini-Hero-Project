using System.Collections.Generic;
using System.Linq;
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

    public List<AbilityData> abilities;

    public int activeAbility1 = -1;
    public int activeAbility2 = -1;

    public bool hasAbilities = false;

    public Dictionary<AbilityData.AbilityEnum, UpgradeMask> abilityUpgrades;

    public AbilityData secondaryAbility;
    
    [SerializeField] private Color[] mainColors = new Color[3];
    [SerializeField] private Color[] secColors = new Color[3];

    
    public void SelectAbility(GameObject abilityObject, int position) {
        if (position >= abilities.Count) return;
        abilities[position] = new AbilityData{abilityGameObject = abilityObject};
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

    public AbilityData AddAbilityFromClass(AbilityData.AbilityEnum abilityEnum, bool fromMain) {
        var powersClass = fromMain ? mainPowers : secondaryPowers;
        
        var ability = powersClass.GetPowerByEnum(abilityEnum);
        if (ability == null) return null;
        if (!fromMain) secondaryAbility = ability;
        
        abilities.Add(ability);
        
        FirstSetUpAbility(abilities.Count-1);

        return ability;
    }
    
    public AbilityData AddAbilityFromClass(int abilityIndex, bool fromMain) {
        var powersClass = fromMain ? mainPowers : secondaryPowers;
        
        var ability = powersClass.GetAbilityAt(abilityIndex);
        if (ability == null) return null;

        if (!fromMain) secondaryAbility = ability;
        
        abilities.Add(ability);
        
        FirstSetUpAbility(abilities.Count-1);

        return ability;
    }

    public void SetAbilityAsActive(int positionInList, int whichActive) {
        if(positionInList >= abilities.Count) return;

        switch (whichActive) {
            case 1:
                activeAbility1 = positionInList;
                break;
            case 2: 
                activeAbility2 = positionInList;
                break;
        }
    }
    
    public void SetAbilityAsActive(AbilityData.AbilityEnum abilityType, int whichActive) {
        var abilityIndex = abilities.FindIndex(ability => ability.abilityEnum == abilityType);
        if(abilityIndex == -1) return;

        switch (whichActive) {
            case 1:
                activeAbility1 = abilityIndex;
                break;
            case 2: 
                activeAbility2 = abilityIndex;
                break;
        }
    }


    public void Init() {
        if (abilities.Count > 1) {
            if (activeAbility1 == -1) {
                activeAbility1 = activeAbility2 == 0 ? 1 : 0;
            }

            if (activeAbility2 == -1) {
                activeAbility2 = activeAbility1 == 0 ? 1 : 0;
            }
        }
        else if(abilities.Count == 1) {
            if (activeAbility1 == -1) activeAbility1 = 0;
        }
        
    }


    private void FirstSetUpAbility(int pos) {
        if (activeAbility1 == -1) {
            activeAbility1 = pos;
        }
        else if(activeAbility2 == -1) {
            activeAbility2 = pos;
        }
    }


    public AbilityData GetActive(int which) {
        return which == 1 ? abilities[activeAbility1] : abilities[activeAbility2];
    }

    public bool IsAbilityUnlocked(AbilityData ability) {
        return abilities.Exists(power => power.name == ability.name);
    }

    public Color[] GetColorsOfAbility(AbilityData ability) {
        if (ability.abilityEnum == secondaryAbility.abilityEnum) {
            return secColors;
        }

        return mainColors;
    }


    public void Clear() {
        abilities.Clear();
        activeAbility1 = -1;
        activeAbility2 = -1;
    }


    public void SetStartingAbilities(int mainAbilityIndex, int secondaryAbilityIndex) {
        Clear();
        AddAbilityFromClass(mainAbilityIndex, true);
        AddAbilityFromClass(secondaryAbilityIndex, false);
    }

    public void SetSubColor(Color c,bool isMain, int sub) {
        if (isMain) {
            mainColors[sub] = c;
        }
        else {
            secColors[sub] = c;
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

        secondaryAbility = secondaryPowers.GetPowerByEnum((AbilityData.AbilityEnum) ser.secAbilityNameEnum);

        for (int i = 0; i < ser.abilityNamesEnum.Length; i++) {
            if (ser.abilityNamesEnum[i] != ser.secAbilityNameEnum) {
                abilities.Add(mainPowers.GetPowerByEnum((AbilityData.AbilityEnum)ser.abilityNamesEnum[i]));
            }
            else {
                abilities.Add(secondaryAbility);
            }
        }

        activeAbility1 = ser.active1;
        activeAbility2 = ser.active2;
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
        // public bool[][] abilityUpgrades;
        
        public int secAbilityNameEnum;

        public float[] mainColors;
        public float[] secondColors;

        public int active1;
        public int active2;

        public SerializedPowers(CharacterPowers other) {
            mainPowerName = other.mainPowers.GetName();
            secPowerName = other.secondaryPowers.GetName();
            
            abilityNamesEnum = new int[other.abilities.Count];
            // abilityUpgrades = new bool[other.abilities.Count][];
            for (int i = 0; i < other.abilities.Count; i++) {
                abilityNamesEnum[i] = (int)other.abilities[i].abilityEnum;
                // abilityUpgrades[i] = other.abilities[i].unlockes;
            }

            secAbilityNameEnum = (int)other.secondaryAbility.abilityEnum;

            mainColors = Utils.ColorsArrayToFloatArray(other.mainColors);
            secondColors = Utils.ColorsArrayToFloatArray(other.secColors);

            active1 = other.activeAbility1;
            active2 = other.activeAbility2;
        }
    }
}
