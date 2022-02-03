using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * Abilities are the Abilities the player and enemies can use during the game.
 * Each ability inherits from this class.
 */
public abstract class Ability2: Ability
{
    [SerializeField] protected float energyRequiredWindUp;
    [SerializeField] protected float energyRequiredHang;
    [SerializeField] protected float energyRequiredWindDown;
    [SerializeField] protected float energyCostReduction;

    protected bool _reducedEnergy = false;
    protected bool _reducedCooldown = false;

    [SerializeField] protected float cooldownReduction;

    [SerializeField] protected Color[] _colors;

    protected bool isToggle;

    [SerializeField] protected float windUpTime;
    [SerializeField] protected float hangTime;
    [SerializeField] protected float windDownTime;

    protected states curentState = states.off;

    protected List<Coroutine> coroutines = new List<Coroutine>();

    protected enum states{
        windUp, release, hang, windDown, off
    }

    protected List<Transform> angledParts = new List<Transform>();
    private Dictionary<Transform, float> angledPartsDefaultAngles = new Dictionary<Transform, float>();

    protected bool released = false;

    protected float curTimer;


    
    // Uses the ability
    public virtual void UseAbility()
    {
        if (!AbilityOn && CanUseAbilityAgain() && UseRequiredEnergy()) {
            AbilityOn = true;
            AbilityOnInvoke();
            AdditionalUseAbility();
            coroutines.Add(StartCoroutine(WindUp()));
        }
        else if(isToggle && AbilityOn) {
            // ability off
        }
    }

    public virtual void AdditionalUseAbility(){}

    protected virtual IEnumerator WindUp(){
        curentState = states.windUp;

        curTimer = windUpTime;
        while (true) { 
            if (curTimer <= 0 && curTimer > -10) break;
            if(AditionalWindUp(curTimer)) break;
            if(!UseRequiredEnergy()) break;
            yield return null;
            curTimer -= Time.deltaTime;
        }
        
        Activate();
    }

    protected virtual bool AditionalWindUp(float currentTimer){
        return false;
    }

    protected virtual void Activate(){
        curentState = states.release;
        coroutines.Add(StartCoroutine(Hang()));
    }

    protected virtual IEnumerator Hang(){
        curentState = states.hang;

        curTimer = hangTime;
        while (true) {
            if (curTimer <= 0 && curTimer > -10) break;
            if(AditionalHang(curTimer)) break;
            if(!UseRequiredEnergy()) break;
            yield return null;
            curTimer -= Time.deltaTime;
        }
        
        Deactivate();
    }
    protected virtual bool AditionalHang(float currentTimer){
        return false;
    }

    protected virtual void Deactivate(){
        coroutines.Add(StartCoroutine(WindDown()));
    }

    protected virtual IEnumerator WindDown(){
        curentState = states.hang;
        var timer = windDownTime;
        while (true) {
            if (timer <= 0 && timer > -10) break;
            if(AditionalWindDown(timer)) break;
            if(!UseRequiredEnergy()) break;
            yield return null;
            timer -= Time.deltaTime;
        }
        
        SetAbilityOff();
    }
    protected virtual bool AditionalWindDown(float currentTimer){
        return false;
    }

    // Sets the ability as off.
    public override void SetAbilityOff(){
        
        if(!AbilityOn) return;
        
        curentState = states.off;

        AbilityOn = false;

        ResetBodyAngles();
        
        foreach(var coroutine in coroutines){
            if (coroutine != null)
            StopCoroutine(coroutine);
        }
        coroutines = new List<Coroutine>();

        AditionalAbilityOff();
        
        Manager.EnableManager();

        var coolDown = _reducedCooldown ? abilityCooldown * cooldownReduction : abilityCooldown;
        NextCanUse = Time.time + coolDown;

        AbilityOffInvoke();
    }

    protected virtual void AditionalAbilityOff(){
    }

    protected virtual void ResetBodyAngles(){
        foreach(var item in angledPartsDefaultAngles)
        {
            bodyAngler.RotatePart(item.Key, item.Value);
        }
    }


    // Start input to use ability
    public virtual void UseAbilityStart() {
        UseAbility();
    }

    // Release input of the ability
    public virtual void UseAbilityRelease() {
        if(!AbilityOn || released) return;
        released = true;
    }

    // Run when damage is taken by the player. (Usually turns off the power)
    protected override void OnDamageTaken(object o, float damageAmount) {
        SetAbilityOff();
    }

    public virtual bool UseRequiredEnergy(){
        float energyCost = 0;
        switch(curentState){
            case states.off: energyCost = energyRequired; break;
            case states.windUp: energyCost = energyRequiredWindUp * Time.deltaTime; break;
            case states.hang: energyCost = energyRequiredHang * Time.deltaTime; break;
            case states.windDown: energyCost = energyRequiredWindDown * Time.deltaTime; break;
        }
        if(_reducedEnergy) energyCost *= energyCostReduction;
        return CharacterStats.UseEnergy(energyCost);
    }

    protected float normalizedTimer(float currentTimer, float fromTime=0){
        if(fromTime == 0){
            switch(curentState){
                case states.windUp: fromTime = windUpTime; break;
                case states.hang: fromTime = hangTime; break;
                case states.windDown: fromTime = windDownTime; break;
                default: return currentTimer;
            }
        }
        return currentTimer/fromTime;
        
    }

    /*
    Here is a template for starting a new Ability:

    protected override void UnlockAbilityMap() {
        _reducedEnergy = upgrades[0];
        _reducedCooldown = upgrades[1];
        _ = upgrades[2];
        _ = upgrades[3];
    }

    public override void UseAbility(Vector3 direction){
        base.UseAbility();
    }

    protected override void AdditionalInit(){
        
    }

    protected override bool AditionalWindUp(float currentTimer){
        return false;
    }

    protected override void Activate(){
        base.Activate();
    }

    protected override bool AditionalHang(float currentTimer){
        return false;
    }

    protected override void Deactivate(){
        base.Deactivate();
    }

    protected override bool AditionalWindDown(float currentTimer){
        return false;
    }

    protected override void AditionalAbilityOff(){
    }
    
    protected override void OnDamageTaken(object o, float damageAmount) {
        base.OnDamageTaken(o, damageAmount);
    }
    */

}
