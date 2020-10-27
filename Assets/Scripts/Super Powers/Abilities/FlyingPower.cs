using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Allows the character to fly.
 * When flying energy regen is off.
 * Taking damage while flying will cost energy.
 *
 * Flying is done in practice in the character controller. This ability just turn that mode on/off.
 */
public class FlyingPower : Ability
{
    [SerializeField] private float energyCostOnDamage;
    private float _originalGravityScale;

    protected override void AdditionalInit() {
        return;
    }

    protected override void OnDamageTaken(object o, float damageAmount) {
        CharacterStats.UseEnergy(energyCostOnDamage);
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn && CharacterStats.UseEnergy(EnergyRequired())) {
            AbilityOn = true;
            AbilityOnInvoke();
            Manager.Fly(true);
        }
        else {
            SetAbilityOff();
        }
    }

    private void Update() {
        AbilityOn = Manager.IsFlying();
        if (!AbilityOn) {
            AbilityOffInvoke();
        }

        if (AbilityOn) {
            CharacterStats.UseEnergy(CharacterStats.GetCharacterStats().EnergyRegen*Time.deltaTime);   
        }
    }

    public override bool CanUseAbilityAgain() {
        return true;
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;
        
        AbilityOn = false;
        AbilityOffInvoke();
        Manager.Fly(false);
    }
}
