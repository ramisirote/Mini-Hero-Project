using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/*
 * Power Instance is the mono behaviour that links up the instance of the player and the Character Powers.
 * This class creates the ability objects and inits the abilities scripts, attaching them to the player.  
 */
public class PowerInstanceold : MonoBehaviour
{
    public CharacterPowers powers;

    
    private int active1 = -1;
    private int active2 = -1;
    [SerializeField] private PlayerManager manager;

    private List<Ability> abilitiesList = new List<Ability>();

    private void Awake() {
        manager = GetComponent<PlayerManager>();

        if (!powers.hasAbilities || !manager) return;
        
        powers.Init();
        active1 = powers.activeAbility1;
        active2 = powers.activeAbility2;
        
        var pos = transform.position;

        foreach (var abilityData in powers.abilities) {
            SetUpAbility(abilityData);
        }
        
        if(active1 > -1) manager.SetAbility(abilitiesList[active1], 1);
        if(active2 > -1) manager.SetAbility(abilitiesList[active2], 2);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            ClearPowers();
        }
    }

    public void RotateAbility(int whichOne) {
        switch (whichOne) {
            case 1: 
                if(active1 > -1) SetActiveAbility(active1 + 1, 1); 
                break;
            case 2: 
                if(active2 > -1) SetActiveAbility(active2 + 1, 2); 
                break;
        }
    }

    public void SetActiveAbility(AbilityDataOld.AbilityEnum abilityType, int whichOne) {
        powers.SetAbilityAsActive(abilityType, whichOne);

        switch (whichOne) {
            case 1:
            {
                int index = powers.activeAbility1;
                abilitiesList[active1].SetAbilityOff();
                active1 = index;
                if (active1 == active2) {
                    SetActiveAbility(active2+1, 2);
                }
                manager.SetAbility(abilitiesList[active1], 1);
                break;
            }
            case 2:
            {
                int index = powers.activeAbility2;
                abilitiesList[active2].SetAbilityOff();
                active2 = index;
                if (active1 == active2) {
                    SetActiveAbility(active1+1, 1);
                }
                manager.SetAbility(abilitiesList[active2], 2);
                break;
            }
        }
    }

    private void SetActiveAbility(int index, int whichOne) {
        if (abilitiesList.Count <= 2) {
            SwapActives();
        }
        switch (whichOne) {
            case 1:
            {
                abilitiesList[active1].SetAbilityOff();
            
                active1 = index%abilitiesList.Count;
                if (active1 == active2) {
                    active1 = (active1+1)%abilitiesList.Count;
                }
            
                powers.SetAbilityAsActive(active1, 1);
                manager.SetAbility(abilitiesList[active1], 1);
                break;
            }
            case 2:
            {
                abilitiesList[active2].SetAbilityOff();
            
                active2 = (active2+1)%abilitiesList.Count;
                if (active1 == active2) {
                    active2 = (active2+1) % abilitiesList.Count;
                }
                
            
                powers.SetAbilityAsActive(active2, 2);
                manager.SetAbility(abilitiesList[active2], 2);
                break;
            }
        }
    }

    private void SwapActives() {
        if(active1 > -1) abilitiesList[active1].SetAbilityOff();
        if(active2 > -1) abilitiesList[active2].SetAbilityOff();
        manager.ClearPowers();
        
        var temp = active1;
        active1 = active2;
        active2 = temp;
        powers.SetAbilityAsActive(active2, 2);
        powers.SetAbilityAsActive(active1, 1);
        if(active1 > -1) manager.SetAbility(abilitiesList[active1], 1);
        if(active2 > -1) manager.SetAbility(abilitiesList[active2], 2);
    }

    private void SetUpAbility(AbilityDataOld abilityData) {
        var abilityObject = Instantiate(abilityData.abilityGameObject, transform.position, Quaternion.identity);
        var ability = abilityObject.GetComponent<Ability>();
        abilitiesList.Add(ability);
        Color[] abilityColors = powers.GetColorsOfAbility(abilityData);
        ability.Init(gameObject,abilityColors, powers.GetUnlocksArr(abilityData.abilityEnum));
    }


    public void AddPowerAbility(AbilityDataOld.AbilityEnum newPowerEnum, bool fromMainPowers) {
        var abilityData = powers.AddAbilityFromClass(newPowerEnum, fromMainPowers);
        SetUpAbility(abilityData);
        

        switch (abilitiesList.Count) {
            case 1: 
                active1 = 0; 
                manager.SetAbility(abilitiesList[active1], 1);
                break;
            case 2: 
                active2 = 1;
                manager.SetAbility(abilitiesList[active2], 2); 
                break;
        }
    }

    // public void UpgradeAbility(AbilityData.AbilityEnum abilityEnum, int index) {
    //     powers.UpgradeAbility(abilityEnum, index);
    // }
    
    public void UpgradeAbility(int index) {
        var abilityData = powers.GetActive(1);
        powers.UpgradeAbility(abilityData.abilityEnum, index);
        abilitiesList[active1].Upgrade(index);
    }

    public void SetPowerClass(PowersClass powersClass, bool asMain) {
        powers.SetPowerClass(powersClass, asMain);
    }


    public bool IsAbilityUnlocked(AbilityDataOld ability) {
        return powers.IsAbilityUnlocked(ability);
    }

    public void ClearPowers() {
        powers.Clear();
        abilitiesList.Clear();
        active1 = -1;
        active2 = -1;
        manager.ClearPowers();
    }
}
