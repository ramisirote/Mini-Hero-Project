using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerCustimizer : MonoBehaviour
{
    public static powerCustimizer instance;

    private CharacterPowerManager powerManager;
    private AbilityManager abilityManager;

    private PowerClassData.PowerClasses currentMainPower;
    private AbilityStatic.AbilityEnum currentMainAbility;
    private AbilityStatic.AbilityEnum currentSecondAbility;
    private SelectTypes currentSelectType;

    public event EventHandler<GameObject> onSelectChenged;
    public event EventHandler onSelectTypeChange;

    public enum SelectTypes{
        powers, abilities, powerAbilities, colors
    }


    // Start is called before the first frame update
    private void Awake() {
        if (instance == null){
            instance = this;
        }
        powerManager = GameControler.GetPowerManager();
        abilityManager = AbilityManager.instance;
        currentMainAbility = powerManager.abilities[0].id;
        currentSecondAbility = powerManager.GetSecondaryAbilityData().id;
        currentMainPower = powerManager.GetMainPower();
    }

    public PowerClassData.PowerClasses getCurrentMainPower(){
        return currentMainPower;
    }

    public AbilityStatic.AbilityEnum getCurrentMainAbility(){
        return currentMainAbility;
    }

    public AbilityStatic.AbilityEnum getCurrentSecondAbility(){
        return currentSecondAbility;
    }

    public void SelectChanged(GameObject selected, SelectTypes type){
        onSelectChenged?.Invoke(this, selected);
        if(currentSelectType != type){
            onSelectTypeChange?.Invoke(this, null);
        }
        currentSelectType = type;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
