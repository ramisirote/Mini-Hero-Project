using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager instance = null;

    public void Awake() {
        if(instance == null){
            instance = this;
        }
    }

    public List<AbilityStatic> abilityLibrary;
    public List<PowerClassData> powerClassDatas;

    public List<AbilityStatic> GetAbiitiesExceptClass(PowerClassData.PowerClasses classId){
        return abilityLibrary.FindAll(ability => ability.powerClass != classId);
    }

    public AbilityStatic GetAbilityStatic(AbilityStatic.AbilityEnum id){
        return abilityLibrary.Find(ability => (int)ability.id == (int)id);
    }

    public PowerClassData GetPowerClass(PowerClassData.PowerClasses id){
        return powerClassDatas.Find(powerData => (int)powerData.id == (int)id);
    }

    public List<AbilityStatic> GetAbilitiesForClass(PowerClassData.PowerClasses classId){
        return abilityLibrary.FindAll(ability => ability.powerClass == classId);
    }
}

[System.Serializable]
public class PowerClassData
{
    public enum PowerClasses{
        Energy, Fire, Strength, Speed, Flight, Telekinesis, MartialArts, Custome,
        Single
    }
    public string name;

    public PowerClasses id;

    public Sprite icon;

    public string description;

}

[System.Serializable]
public class AbilityStatic
{
    public enum AbilityEnum
    {
        FlameThrower, FireBody, FireBurst, FireBall, Blast, Flying, Teleportation, SuperSpeed, Strength, Invisibility, EnergyBurst, EnergyCharge,
        EnergyBeam, SpeedDash, StrengthSmash, TornadoRun, BullRun, FlurryPunches, MartialArts, PickupThrow, Detonate,
        SuperPunch, Telekinesis, TelekineticBurst, TelekineticShield, TelekineticBlast, Acrobatics, StunStrike, CounterSrtike
    }

    public string name;

    
    public AbilityEnum id;

    public GameObject abilityObject;

    public Sprite icon;

    public string description;

    public PowerClassData.PowerClasses powerClass;

    public UpgradeData[] upgradeData = new UpgradeData[4];

    public Color[] defaultColors = new Color[3];
}

[System.Serializable]
public class UpgradeData
{
    public string name = "NA";
    public string description = "NA";

    public Sprite icon;
}
