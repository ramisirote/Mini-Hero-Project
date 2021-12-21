using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerCustimizer : ColorCustimizer
{
    public static powerCustimizer instance;

    private CharacterPowerManager powerManager;
    private AbilityManager abilityManager;

    private PowerClassData.PowerClasses currentMainPower;
    private AbilityStatic currentMainAbility;
    private AbilityStatic currentSecondAbility;
    private SelectTypes currentSelectType = SelectTypes.powers;

    public PowerClassData.PowerClasses defaultClass;
    public AbilityStatic.AbilityEnum defaultSecondaryAbility;

    [HideInInspector] public powerSelect curentSelected;

    private Dictionary<PowerClassData.PowerClasses, AbilityStatic> lastSelectedForClass = new Dictionary<PowerClassData.PowerClasses, AbilityStatic>();

    public event EventHandler<GameObject> onSelectChenged;
    public event EventHandler<SelectTypes> onSelectTypeChange;

    private Dictionary<SelectTypes, List<Color>> typesColors = new Dictionary<SelectTypes, List<Color>>();

    // public event EventHandler onNewPowerSelected;

    public event EventHandler OnNewSelected;
    // public event EventHandler onNewMainAbilitySelected;

    public enum SelectTypes{
        powers, abilities, powerAbilities, mainColors, secColors, none
    }


    // Start is called before the first frame update
    private void Awake() {
        if (instance == null){
            instance = this;
        }
        powerManager = GameControler.GetPowerManager();
        abilityManager = AbilityManager.instance;
        if(powerManager.IsSetUp()){
            currentMainAbility = powerManager.abilities[0].abilityData;
            currentSecondAbility = powerManager.GetSecondaryAbilityData();
            currentMainPower = powerManager.GetMainPower();
        }
        else{
            currentMainPower = defaultClass;
            currentSecondAbility = abilityManager.GetAbilityStatic(defaultSecondaryAbility);
            currentMainAbility = abilityManager.GetAbilitiesForClass(currentMainPower)[0];
        }
        typesColors[SelectTypes.secColors] = new List<Color>(currentSecondAbility.defaultColors);
        typesColors[SelectTypes.mainColors] = new List<Color>(currentMainAbility.defaultColors);
    }

    public PowerClassData.PowerClasses getCurrentMainPower(){
        return currentMainPower;
    }

    public void setCurrentMainPower(PowerClassData.PowerClasses classId){
        currentMainPower = classId;
        var abilites = abilityManager.GetAbilitiesForClass(currentMainPower);
        lastSelectedForClass[currentMainPower] = lastSelectedForClass.ContainsKey(currentMainPower) 
                                                    ? lastSelectedForClass[currentMainPower] : abilites[0];
        currentMainAbility = lastSelectedForClass[currentMainPower];
        if(currentSecondAbility.powerClass == classId){
            currentSecondAbility = abilityManager.GetAbilityStatic(defaultSecondaryAbility);
        }
        OnNewSelected?.Invoke(this, null);
    }

    public AbilityStatic getCurrentMainAbility(){
        return currentMainAbility;
    }

    public AbilityStatic getCurrentSecondAbility(){
        return currentSecondAbility;
    }

    public AbilityStatic getCurrentAbility(){
        if (currentSelectType == SelectTypes.abilities){
            return getCurrentSecondAbility();
        } else {
            return getCurrentMainAbility();
        }
    }

    public SelectTypes getCurrentSelectType(){
        return currentSelectType;
    }

    public void setCurrentSelectType(SelectTypes newType){
        currentSelectType = newType;
    }

    public void setAbility(AbilityStatic abilityData){
        if (currentSelectType == SelectTypes.abilities){
            currentSecondAbility = abilityData;
        } else {
            currentMainAbility = abilityData;
        }
        OnNewSelected?.Invoke(this, null);    
    }

    public void SelectChanged(GameObject selected, powerSelect select){
        curentSelected = select;
        onSelectChenged?.Invoke(this, selected);
        if(currentSelectType != select.selectType){
            currentSelectType = select.selectType;
            onSelectTypeChange?.Invoke(this, select.selectType);
        }
    }

    public override Color GetCurrentColor() {
        var type = currentSelectType == SelectTypes.powers ? SelectTypes.powerAbilities : currentSelectType;
        return typesColors[type][currentColorIndex];
    }

    public override void ChangeSelectedColor(Color newColor, int selectedRainbowIndex=-1){
        var type = currentSelectType == SelectTypes.powers ? SelectTypes.powerAbilities : currentSelectType;
        base.ChangeSelectedColor(newColor, selectedRainbowIndex);
        typesColors[type][currentColorIndex] = newColor;
    }

    public override void ChangeSelectedColorIndex(int index, Color color){
        var type = currentSelectType == SelectTypes.powers ? SelectTypes.powerAbilities : currentSelectType;
        typesColors[type][index] = color;
        base.ChangeSelectedColorIndex(index, color);
    }

    public void SetPowerManagerPowers(){
        powerManager.ClearPowers();
        powerManager.AddNewAbilities(abilityManager.GetAbilitiesForClass(currentMainPower),
                                    colors: typesColors[SelectTypes.mainColors].ToArray(), 
                                    isMain: true);
        powerManager.UnlockAbility(currentMainAbility.id);
        powerManager.AddNewAbility(currentSecondAbility, unlocked: true, colors: typesColors[SelectTypes.secColors].ToArray());
        powerManager.SaveAbilities();
    }
}
