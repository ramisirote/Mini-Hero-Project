using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class abilityCard : MonoBehaviour
{
    public Image icon;
    public Button button;
    public TextMeshProUGUI nameText;

    public Outline[] outlines;

    [HideInInspector] public bool isSelected;

    private powerCustimizer powerCustimizer;
    private CharacterPowerManager powerManager;

    private AbilityStatic abilityData;

    private AbilityManager abilityManager;
    

    // Start is called before the first frame update
    void OnEnable()
    {
        powerCustimizer = powerCustimizer.instance;
        abilityManager = AbilityManager.instance;
        foreach(var outline in outlines) isSelected = outline.enabled = false;
        powerCustimizer.OnNewSelected += OnNewAbilitySelected;
    }

    private void OnDisable() => powerCustimizer.OnNewSelected -= OnNewAbilitySelected;

    private void OnNewAbilitySelected(object sender, object args){
        foreach(var outline in outlines) isSelected = outline.enabled = powerCustimizer.getCurrentAbility().id == abilityData.id;
    }
    
    public void init(AbilityStatic abilityData){
        icon.sprite = abilityData.icon;
        nameText.text = abilityData.name;
        this.abilityData = abilityData;
        foreach(var outline in outlines) isSelected = outline.enabled = powerCustimizer.getCurrentAbility().id == abilityData.id;

        button.onClick.AddListener(Select);
    }
    
    private void Select(){
        powerCustimizer.setAbility(abilityData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
