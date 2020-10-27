using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This scriptable object contains all the super powers in the game.
 * Used for loading, the power's name can be saved, then gotten from here. 
 */
[CreateAssetMenu(fileName = "SuperPowerLibrary", menuName = "CharacterPowers/SuperPowerLibrary", order = 1)]
public class SuperPowerLibrary : ScriptableObject
{
    public List<PowersClass> powersClasses;

    public List<GameObject> abilityGameObject;


    public PowersClass GetPowerClass(string powerName) {
        return powersClasses.Find(p => p.GetName() == powerName);
    }
}
