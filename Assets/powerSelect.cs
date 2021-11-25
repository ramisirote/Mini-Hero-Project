using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class powerSelect : MonoBehaviour
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

            case powerCustimizer.SelectTypes.colors: break;
        }
        outline = gameObject.GetComponent<Outline>();
        powerCustimizer.onSelectChenged += OnSeletChanged;
        if (isDefaultSelected) ClickSelect();
        button.onClick.AddListener(ClickSelect);
    }

    private void OnSeletChanged(object sender, GameObject selected)
    {
        if (selected != gameObject)
        {
            outline.enabled = false;
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

    private void SetUpAbilitySelect(AbilityStatic.AbilityEnum abilityId)
    {
        var abilityData = abilityManager.GetAbilityStatic(abilityId);
        icon.sprite = abilityData.icon;
        nameText.text = abilityData.name;
        Debug.Log(nameText.text);
    }

    public void ClickSelect()
    {
        outline.enabled = true;
        powerCustimizer.SelectChanged(gameObject, selectType);
    }
}
