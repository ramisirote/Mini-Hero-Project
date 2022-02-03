using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerAbilityCard : MonoBehaviour
{
    // public Button slideButton;
    // public Image slideArrow;
    // public Transform slideWindow;

    private float startingHight;

    public abilityCard[] abilityCards;

    public Image icon;
    public TextMeshProUGUI nameText;

    private powerCustimizer powerCustimizer;
    private CharacterPowerManager powerManager;

    public void init(PowerClassData.PowerClasses powerClass){
        var powerData = AbilityManager.instance.GetPowerClass(powerClass);
        icon.sprite = powerData.icon;
        nameText.text = powerData.name;
        var abilities = AbilityManager.instance.GetAbilitiesForClass(powerData.id);
        for(var i=0; i<abilities.Count; i++){
            abilityCards[i].init(abilities[i]);
        }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        powerCustimizer = powerCustimizer.instance;
        powerManager = GameControler.GetPowerManager();
        // startingHight = this.transform.localScale.y;
        // this.transform.localScale = this.transform.localScale * Vector2.right;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
