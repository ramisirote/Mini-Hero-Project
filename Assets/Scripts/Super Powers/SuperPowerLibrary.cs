using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SuperPowerLibrary", menuName = "CharacterPowers/SuperPowerLibrary", order = 1)]
public class SuperPowerLibrary : ScriptableObject
{
    public List<PowersClass> powersClasses;

    public List<GameObject> abilityGameObject;


    public PowersClass GetPowerClass(string powerName) {
        return powersClasses.Find(p => p.GetName() == powerName);
    }
}
