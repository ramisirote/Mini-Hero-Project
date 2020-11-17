using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MartialArts : Ability
{
    [SerializeField] private float damageMult;
    [SerializeField] private MartialArtAttaker martialArtAttaker;
    [SerializeField] private float cameraShake;
    [SerializeField] private Vector2[] pushVectors = new Vector2[5];
    [SerializeField] private float[] attackCooldowns = new float[4];
    [SerializeField] private float hitJump;
    
    private AttackManagerBase _attackDefault;
    
    protected override void AdditionalInit() {
        _attackDefault = parentCharacter.GetComponent<AttackManagerBase>();
        martialArtAttaker.Init(Animator, Controller, CharacterStats,attackCooldowns, pushVectors, damageMult, 
            cameraShake,hitJump, IsPlayer);
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn) {
            SetPowerOn();
        }
        else {
            SetAbilityOff();
        }
    }

    private void SetPowerOn() {
        AbilityOn = true;
        

        Manager.SetAttackManager(martialArtAttaker);
        AbilityOnInvoke();
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;
        AbilityOn = false;
        
        
        Manager.SetAttackManager(_attackDefault);
        AbilityOffInvoke();
    }
}
