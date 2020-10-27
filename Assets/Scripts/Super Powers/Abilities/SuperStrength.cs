using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperStrength : Ability
{
    [SerializeField] private float damageMult;
    [SerializeField] private float sizeMult;
    [SerializeField] private float headDownSet;
    [SerializeField] private GameObject thrower;
    [SerializeField] private SuperStrangthAttack strengthAttack;
    [SerializeField] private float attackCooldown;
    private EffectPoints _effectPoints;
    private Transform torso;
    private Transform head;
    private AttackManagerBase _attackDefault;
    // private SuperStrangthAttack _superAttack;
    
    protected override void AdditionalInit() {
        _effectPoints = parentCharacter.GetComponent<EffectPoints>();
        if (_effectPoints) {
            torso = _effectPoints.GetJointObject(Refarences.BodyJoints.ChestLower).transform.parent;
            head = _effectPoints.GetJointObject(Refarences.BodyJoints.Head).transform.parent;
        }

        _attackDefault = parentCharacter.GetComponent<AttackManagerBase>();
        strengthAttack.Init(parentCharacter.GetComponent<Animator>(), Controller, CharacterStats,
                            Manager, attackCooldown, thrower, damageMult);
    }

    // private void FixedUpdate() {
    //     if (PowerOn) {
    //         strengthAttack.direction = Manager.GetPowerTargetDirection();
    //     }
    // }

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
        if (torso) {
            torso.localScale *= sizeMult;
            
            head.localScale /= sizeMult;
            
            var torsoPos = torso.transform.position;
            torsoPos.y += headDownSet;
            torso.transform.position = torsoPos;
            
            var headPos = head.transform.position;
            headPos.y -= headDownSet;
            head.transform.position = headPos;
        }

        Manager.SetAttackManager(strengthAttack);
        AbilityOnInvoke();
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;
        AbilityOn = false;
        if (torso) {
            var headPos = head.transform.position;
            headPos.y += headDownSet;
            head.transform.position = headPos;
            
            var torsoPos = torso.transform.position;
            torsoPos.y -= headDownSet;
            torso.transform.position = torsoPos;
            
            head.localScale *= sizeMult;
            
            torso.localScale /= sizeMult;
        }
        
        Manager.SetAttackManager(_attackDefault);
        AbilityOffInvoke();
    }
}
