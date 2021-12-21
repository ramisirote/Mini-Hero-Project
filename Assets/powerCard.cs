using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class powerCard : MonoBehaviour
{
    public Image icon;
    public Button button;
    public TextMeshProUGUI nameText;

    public Outline[] outlines;

    [HideInInspector] public bool isSelected;

    private powerCustimizer powerCustimizer;

    private PowerClassData.PowerClasses classId;

    private AbilityManager abilityManager;


    // Start is called before the first frame update
    void OnEnable()
    {
        powerCustimizer = powerCustimizer.instance;
        abilityManager = AbilityManager.instance;
        foreach(var outline in outlines) isSelected = outline.enabled = false;
        powerCustimizer.OnNewSelected += OnNewPowerSelected;
    }

    private void OnDisable() {
        powerCustimizer.OnNewSelected -= OnNewPowerSelected;
    }
    
    public void init(PowerClassData powerData){
        icon.sprite = powerData.icon;
        nameText.text = powerData.name;
        classId = powerData.id;
        foreach(var outline in outlines){ 
            isSelected = outline.enabled = powerCustimizer.getCurrentMainPower() == classId;
        }

        button.onClick.AddListener(Select);
    }
    
    private void Select(){
        powerCustimizer.setCurrentMainPower(classId);
    }

    private void OnNewPowerSelected(object sender, object args){
        foreach(var outline in outlines) isSelected = outline.enabled = powerCustimizer.getCurrentMainPower() == classId;
    }
}
