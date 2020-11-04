using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/*
 * Power Instance is the mono behaviour that links up the instance of the player and the Character Powers.
 * This class creates the ability objects and inits the abilities scripts, attaching them to the player.  
 */
public class PowersInstance : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.E) && active1 > -1) {
            abilitiesList[active1].SetAbilityOff();
            
            active1 = (active1+1)%abilitiesList.Count;
            if(active1 == active2) active1 = (active1+1)%abilitiesList.Count;
            
            powers.SetAbilityAsActive(active1, 1);
            manager.SetAbility(abilitiesList[active1], 1);
        }
        
        if (Input.GetKeyDown(KeyCode.Q) && active2 > -1) {
            abilitiesList[active2].SetAbilityOff();
            
            active2 = (active2+1)%abilitiesList.Count;
            if(active2 == active1) active2 = (active2+1)%abilitiesList.Count;
            
            powers.SetAbilityAsActive(active2, 2);
            manager.SetAbility(abilitiesList[active2], 2);
        }
        
        if (Input.GetKeyDown(KeyCode.P)) {
            ClearPowers();
        }
    }

    private void SetUpAbility(AbilityData abilityData) {
        var abilityObject = Instantiate(abilityData.abilityGameObject, transform.position, Quaternion.identity);
        var ability = abilityObject.GetComponent<Ability>();
        abilitiesList.Add(ability);
        Color[] abilityColors = powers.GetColorsOfAbility(abilityData);
        ability.Init(gameObject,abilityColors[0], abilityColors[1], abilityColors[2]);
    }


    public void AddPowerAbility(AbilityData.AbilityEnum newPowerEnum, bool fromMainPowers) {
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

    public void SetPowerClass(PowersClass powersClass, bool asMain) {
        powers.SetPowerClass(powersClass, asMain);
    }


    public bool IsAbilityUnlocked(AbilityData ability) {
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
