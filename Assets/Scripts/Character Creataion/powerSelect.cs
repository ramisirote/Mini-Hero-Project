using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class powerSelect : ColorSelect
{
    public powerCustimizer.SelectTypes selectType;
    public bool isDefaultSelected;
    public Image icon;
    public TextMeshProUGUI nameText;
    public Button button;
    private Outline outline;
    private powerCustimizer powerCustimizer;
    private AbilityManager abilityManager;


    // Start is called before the first frame update
    void Start()
    {
        powerCustimizer = powerCustimizer.instance;
        abilityManager = AbilityManager.instance;
        switch (selectType)
        {
            case powerCustimizer.SelectTypes.powers:
                SetUpPowerSelect(powerCustimizer.getCurrentMainPower());
                break;

            case powerCustimizer.SelectTypes.powerAbilities:
                SetUpAbilitySelect(powerCustimizer.getCurrentMainAbility());
                break;

            case powerCustimizer.SelectTypes.abilities:
                SetUpAbilitySelect(powerCustimizer.getCurrentSecondAbility());
                break;
            
            default: break;
        }
        outline = gameObject.GetComponent<Outline>();
        powerCustimizer.onSelectChenged += OnSeletChanged;
        if (isDefaultSelected) ClickSelect();
        button.onClick.AddListener(ClickSelect);
        powerCustimizer.OnNewSelected += OnNewSelected;
        if(selectType != powerCustimizer.SelectTypes.powers) base.init();
    }

    private void OnNewSelected(object sender, object args){
        switch (selectType){
            case powerCustimizer.SelectTypes.powers:
                var powerClassData = abilityManager.GetPowerClass(powerCustimizer.getCurrentMainPower());
                if(nameText.text != powerClassData.name){
                    SetUpPowerSelect(powerCustimizer.getCurrentMainPower());
                }
                break;
            case powerCustimizer.SelectTypes.powerAbilities:
                if(nameText.text != powerCustimizer.getCurrentMainAbility().name){
                    SetUpAbilitySelect(powerCustimizer.getCurrentMainAbility());
                }
                break;
            case powerCustimizer.SelectTypes.abilities:
                if(nameText.text != powerCustimizer.getCurrentSecondAbility().name){
                    SetUpAbilitySelect(powerCustimizer.getCurrentSecondAbility());
                }
                break;
        }

    }

    private void OnNewPowerSelected(object sender, object args){
        if(selectType != powerCustimizer.SelectTypes.powers) return;
        SetUpPowerSelect(powerCustimizer.getCurrentMainPower());
    }

    private void OnNewMainAbilitySelected(object sender, object args){
        if(selectType != powerCustimizer.SelectTypes.powerAbilities) return;
        SetUpAbilitySelect(powerCustimizer.getCurrentMainAbility());
    }

    private void OnSeletChanged(object sender, GameObject selected)
    {
        if (selected != gameObject)
        {
            outline.enabled = false;
            foreach(var colorCell in colorCells) colorCell.outline.enabled = false;
        }
        else
        {
            outline.enabled = true;
        }
    }

    private void SetUpPowerSelect(PowerClassData.PowerClasses powerId)
    {
        var powerClassData = abilityManager.GetPowerClass(powerId);
        icon.sprite = powerClassData.icon;
        nameText.text = powerClassData.name;
    }

    private void SetUpAbilitySelect(AbilityStatic abilityData)
    {
        icon.sprite = abilityData.icon;
        nameText.text = abilityData.name;
    }

    public void ClickSelect()
    {
        outline.enabled = true;
        powerCustimizer.SelectChanged(gameObject, this);
    }
    

    protected override List<Color> GetDefaultColors()
    {
        if (selectType == powerCustimizer.SelectTypes.abilities){
            return new List<Color>(powerCustimizer.getCurrentSecondAbility().defaultColors);
        }
        return new List<Color>(powerCustimizer.getCurrentMainAbility().defaultColors);
    }

    private powerCustimizer.SelectTypes SelectTypeToColorSelectType(){
        if (selectType == powerCustimizer.SelectTypes.abilities){
            return powerCustimizer.SelectTypes.secColors;
        }
        return powerCustimizer.SelectTypes.mainColors;
    }

    protected override bool IsCatagorySelected()
    {
        var curType = powerCustimizer.getCurrentSelectType();
        
        return curType == SelectTypeToColorSelectType();
    }

    public override void OnClickSelectCatagory(){
        powerCustimizer.SelectChanged(colorCells[selectedColorIndex].gameObject, this);
        powerCustimizer.setCurrentSelectType(SelectTypeToColorSelectType());
        base.OnClickSelectCatagory();
    }
}
