using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class powerAbilitySelect : MonoBehaviour
{
    public Image powerIcon;
    public TextMeshProUGUI powerName;
    public Button button;

    private Outline outline;
    private powerCustimizer powerCustimizer;
    private AbilityManager abilityManager;

    // Start is called before the first frame update
    void Start()
    {
        powerCustimizer = powerCustimizer.instance;
        abilityManager = AbilityManager.instance;
        SetUpAbilitySelect(powerCustimizer.getCurrentMainAbility());
        outline = gameObject.GetComponent<Outline>();
        powerCustimizer.onSelectChenged += OnSeletChanged;
        ClickSelect();
    }

    private void OnSeletChanged(object sender, GameObject selected){
        if(selected != gameObject){
            outline.enabled = false;
        }
        else{
            outline.enabled = true;
        }
    }

    private void SetUpAbilitySelect(AbilityStatic.AbilityEnum abilityId){
        var abilityData = abilityManager.GetAbilityStatic(abilityId);
        powerIcon.sprite = abilityData.icon;
        powerName.text = abilityData.name;
    }

    public void ClickSelect(){
        outline.enabled = true;
        powerCustimizer.SelectChanged(gameObject, powerCustimizer.SelectTypes.powerAbilities);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
