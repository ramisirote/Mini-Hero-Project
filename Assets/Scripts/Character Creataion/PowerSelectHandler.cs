using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PowerSelectHandler : MonoBehaviour
{

    [SerializeField] private Dropdown mainPowerDropdown;
    [SerializeField] private Dropdown secPowerDropdown;

    [SerializeField] private Dropdown mainAbilityDropDown;
    [SerializeField] private Dropdown secAbilityDropDown;

    [SerializeField] private CharacterPowers powers;

    [SerializeField] private PowersClass[] powersOptions;


    private List<Dropdown.OptionData> allPowerOptions = new List<Dropdown.OptionData>();

    private Dropdown.OptionData mainMissingOption;
    private Dropdown.OptionData secMissingOption;

    private void Awake() {
        
        powers.Clear();

        foreach (var option in powersOptions){
            allPowerOptions.Add(new Dropdown.OptionData(option.GetName(), option.GetIcon()));
        }
        
        SetUpMainOptions();    
        
        SetUpSecOptions();
    }

    private void SetUpMainOptions() {
        mainPowerDropdown.ClearOptions();
        foreach (var option in powersOptions) {
            if (secPowerDropdown.options.Count > 0 
                && option.GetName() == secPowerDropdown.options[secPowerDropdown.value].text) {
                mainMissingOption = new Dropdown.OptionData(option.GetName(), option.GetIcon());
                continue;
            }

            if (option.isSinglePower) {
                continue;
            }
            mainPowerDropdown.options.Add(new Dropdown.OptionData(option.GetName(), option.GetIcon()));
        }
        SelectMainPower();
    }

    private void SetUpAbilityDropDown(PowersClass pc, Dropdown dropdown) {
        dropdown.ClearOptions();

        var abilites = pc.GetAbilites();

        foreach (var ability in abilites) {
            dropdown.options.Add(new Dropdown.OptionData(ability.name.ToString(), ability.icon));
        }
        
        dropdown.RefreshShownValue();
        
        SelectAbility();
    }

    public void SelectAbility() {
        if (mainAbilityDropDown == null || secAbilityDropDown == null) return;
        powers.SetStartingAbilities(mainAbilityDropDown.value, secAbilityDropDown.value);
    }

    private void SetUpSecOptions() {
        secPowerDropdown.ClearOptions();
        foreach (var option in powersOptions) {
            if (mainPowerDropdown.options.Count > 0 
                && option.GetName() == mainPowerDropdown.options[mainPowerDropdown.value].text) {
                secMissingOption = new Dropdown.OptionData(option.GetName(), option.GetIcon());
                continue;
            }
            secPowerDropdown.options.Add(new Dropdown.OptionData(option.GetName(), option.GetIcon()));
        }
        SelectSecPower();
    }


    public void SelectMainPower() {

        PowersClass p = Array.Find(powersOptions,option 
            => option.GetName() == mainPowerDropdown.options[mainPowerDropdown.value].text);
        
        powers.SetPowerClass(p, true);
        
        SetUpAbilityDropDown(p, mainAbilityDropDown);

        if (secMissingOption != null && secMissingOption.text!=p.GetName()) {
            secPowerDropdown.options.Add(secMissingOption);
        }
        secMissingOption = secPowerDropdown.options.Find(option => option.text == p.GetName());
        secPowerDropdown.options.Remove(secMissingOption);
    }
    
    public void SelectSecPower() {

        PowersClass p = Array.Find(powersOptions,option 
            => option.GetName() == secPowerDropdown.options[secPowerDropdown.value].text);
        
        powers.SetPowerClass(p, false);
        SetUpAbilityDropDown(p, secAbilityDropDown);

        if (mainMissingOption != null && mainMissingOption.text!=p.GetName()) {
            mainPowerDropdown.options.Add(mainMissingOption);
        }
        mainMissingOption = mainPowerDropdown.options.Find(option => option.text == p.GetName());
        mainPowerDropdown.options.Remove(mainMissingOption);
    }
}
