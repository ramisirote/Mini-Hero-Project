using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PowersInstance : MonoBehaviour
{
    public CharacterPowers powers;

    
    private int active1 = -1;
    private int active2 = -1;
    [SerializeField] private PlayerManager manager;

    private List<Ability> powersList = new List<Ability>();

    private void Awake() {
        manager = GetComponent<PlayerManager>();

        if (!powers.hasAbilities || !manager) return;
        
        powers.Init();
        active1 = powers.active1;
        active2 = powers.active2;
        
        var pos = transform.position;

        foreach (var pow in powers.abilities) {
            var powObj = Instantiate(pow.abilityGameObg, pos, Quaternion.identity);
            var superPow = powObj.GetComponent<Ability>();
            powersList.Add(superPow);
            superPow.Init(gameObject, pow.color1, pow.color2, pow.color3, pow.unlockes);
        }
        
        if(active1 > -1) manager.SetAbility(powersList[active1], 1);
        if(active2 > -1) manager.SetAbility(powersList[active2], 2);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q) && active1 > -1) {
            powersList[active1].SetAbilityOff();
            
            active1 = (active1+1)%powersList.Count;
            if(active1 == active2) active1 = (active1+1)%powersList.Count;
            
            powers.SetAbilityAsActive(active1, 1);
            manager.SetAbility(powersList[active1], 1);
        }
        
        if (Input.GetKeyDown(KeyCode.E) && active2 > -1) {
            powersList[active2].SetAbilityOff();
            
            active2 = (active2+1)%powersList.Count;
            if(active2 == active1) active2 = (active2+1)%powersList.Count;
            
            powers.SetAbilityAsActive(active2, 2);
            manager.SetAbility(powersList[active2], 2);
        }
        
        if (Input.GetKeyDown(KeyCode.P)) {
            ClearPowers();
        }
    }


    public void AddPowerAbility(PowerAbility.AbilityNames newPowerName, bool fromMainPowers) {
        var pow = powers.AddAbilityFromClass(newPowerName, fromMainPowers);
        
        var powObj = Instantiate(pow.abilityGameObg, transform.position, Quaternion.identity);
        var superPow = powObj.GetComponent<Ability>();
        powersList.Add(superPow);
        
        superPow.Init(gameObject, pow.color1, pow.color2, pow.color3, pow.unlockes);

        switch (powersList.Count) {
            case 1: 
                active1 = 0; 
                manager.SetAbility(powersList[active1], 1);
                break;
            case 2: 
                active2 = 1;
                manager.SetAbility(powersList[active2], 2); 
                break;
        }
    }

    public void SetPowerClass(PowersClass powersClass, bool asMain) {
        powers.SetPowerClass(powersClass, asMain);
    }


    public bool IsAbilityUnlocked(PowerAbility ability) {
        return powers.IsAbilityUnlocked(ability);
    }

    public void ClearPowers() {
        powers.Clear();
        powersList.Clear();
        active1 = -1;
        active2 = -1;
        manager.ClearPowers();
    }
}
