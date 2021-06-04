using System;
using UnityEngine;

[System.Serializable]
public class UpgradeMask
{
    public static int NUMBER_OF_UPGRADES = 4;
    private bool[] unlocks = new bool[NUMBER_OF_UPGRADES];
    
    public UpgradeMask() {
        
        for (int i = 0; i < NUMBER_OF_UPGRADES; i++) {
            unlocks[i] = false;
        }
    }

    public UpgradeMask(bool[] setUnlocks) {
        if (setUnlocks.Length != NUMBER_OF_UPGRADES) {
            Debug.LogWarning("Bad length of upgrades");
        }
        
        for (int i = 0; i < setUnlocks.Length && i < NUMBER_OF_UPGRADES; i++) {
            unlocks[i] = setUnlocks[i];
        }
    }

    public void UnlockUpgrade(int index) {
        if (index < 0 || index > NUMBER_OF_UPGRADES) {
            throw new IndexOutOfRangeException(
                "The given index is not a valid index to unlock. Must be between 0 and "+NUMBER_OF_UPGRADES);
        }

        unlocks[index] = true;
    }

    public bool CheckUnlocked(int index) {
        if (index < 0 || index > NUMBER_OF_UPGRADES) {
            throw new IndexOutOfRangeException(
                "The given index is not a valid index to unlock. Must be between 0 and "+NUMBER_OF_UPGRADES);
        }

        return unlocks[index];
    }

    public bool[] GetUnlocks() {
        return (bool[])unlocks.Clone();
    }
}
