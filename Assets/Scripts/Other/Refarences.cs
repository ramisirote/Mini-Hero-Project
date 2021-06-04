using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refarences
{
    
    public Dictionary<string, EBodyParts> bodyPartKeys = new Dictionary<string, EBodyParts>() {
        {"Head", EBodyParts.Head}, {"Chest", EBodyParts.Chest}, {"ArmL", EBodyParts.ArmL}, {"ArmR", EBodyParts.ArmR},
        {"Waist", EBodyParts.Waist}, {"LegL", EBodyParts.LegL}, {"LegR", EBodyParts.LegR},
        {"Cape", EBodyParts.Cape}, {"Hair", EBodyParts.Hair}, {"Logo", EBodyParts.Logo}, {"Face", EBodyParts.Face},
        {"HairBack", EBodyParts.HairBack}
    };
    
    public enum EBodyParts
    {
        Head, Chest, ArmL, ArmR, Waist, LegL, LegR, Cape, Hair, Logo, Face, HairBack, punch
    }
    
    public enum BodyJoints
    {
        Head, 
        ChestUpper, ChestLower, 
        ArmLUpper, ArmLLower, 
        ArmRUpper, ArmRLower,
        Waist, 
        LegLUpper, LegLLower, LegLFoot, 
        LegRUpper, LegRLower, LegRFoot, 
        CapeBase, Cape1, Cape2, Cape3, Cape4
    }
    
    public enum EGender
    {
        Male, Female
    }
}

public class AnimRefarences
{
    public static string Punch01 = "Punch01";
    public static string Punch02 = "Punch02";
    public static string Punch03 = "Punch03";
    public static string Punch04 = "Punch04";
    public static string Punch05 = "Punch05";
    public static string Flying = "Flying";
    public static string Hit = "Hit";
    public static string Jumping = "jumping";
    public static string Speed = "speed";
    public static string Crouching = "crouching";
    public static string AttackSpeed = "AttackSpeed";
    public static string Dead = "Dead";
    public static string Blast = "Blast";
    public static string Falling = "Falling";
    public static string IsEnemy = "IsEnemy";
    public static string IsFireingContinues = "IsFireingContinues";
    public static string WalkReverse = "WalkReverse";
    public static string Burst = "Burst";
    public static string Dash = "Dash";
    public static string Smash = "Smash";
    public static string TornadoRun = "TornadoRun";
    public static string SpeedMult = "SpeedMult";
    public static string BullRun = "BullRun";
    public static string FlurryPunches = "FlurryPunches";
    public static string StrengthPunch = "StrengthPunch";
    public static string HandsOut = "HandsOut";
    public static string Grab = "Grab";
    public static string Stunned = "Stunned";
    public static string PunchLoad = "PunchLoad";
    //public static string PunchRelease;


    public static void ResetAnimatorBools(Animator anim) {
        anim.SetBool(Flying, false);
        anim.SetBool(Jumping, false);
        anim.SetBool(Crouching, false);
        anim.SetBool(Dead, false);
        anim.SetBool(IsFireingContinues, false);
        anim.SetBool(HandsOut, false);
        anim.SetBool(Stunned, false);
    }
}