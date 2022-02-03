using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthPunch : Ability
{
    [SerializeField] private float damage;
    [SerializeField] private float cameraShake;
    [SerializeField] private GameObject thrower;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float hitRadius;
    
    [Header("Thrower Properties")] 
    [SerializeField] private float throwerDamage;
    [SerializeField] private float flyTime;
    [SerializeField] private float flyForce;

    private Transform _handTransform;
    
    private bool isActive = false;

    protected override void OnDamageTaken(object o, float damageAmount) {
        if(AbilityOn) SetAbilityOff();
    }

    protected override void AdditionalInit() {
        _handTransform = parentCharacter.GetComponent<EffectPoints>().GetPointTransform(Refarences.EBodyParts.ArmR);
    }

    public override void UseAbility(Vector3 direction) {
        if(AbilityOn || !CharacterStats.UseEnergy(energyRequired)) return;
        
        Animator.SetTrigger(AnimRefarences.StrengthPunch);
        Manager.DisableManager();
        Controller.StopHorizontal();
        Manager.FaceTarget();
        AbilityOnInvoke();
        isActive = false;
        AbilityOn = true;
    }


    public override void AnimationTrigger() {
        if(!AbilityOn) return;
        
        if (!isActive) {
            StrengthAttackTrigger();
            isActive = true;
        }
        else {
            SetAbilityOff();
        }
    }

    private void StrengthAttackTrigger() {
        Controller.StopHorizontal();
        var hit = Physics2D.OverlapCircle(_handTransform.position, hitRadius, enemyLayer);
        if (hit) {
            if (hit.GetComponent<CharacterStats>().IsDead()) return;
            
            HitManager.GetTakeDamage(hit.gameObject)?.Damage(damage, Vector2.zero);
            if(IsPlayer) CinemachineShake.Instance.ShakeCamera(cameraShake);
            
            // The thrower handles the push and damage parts of the super strength hit, as well as
            // the collision with other enemies
            hit.GetComponent<IManager>().DisableManager();
            var throwerInstance = Instantiate(thrower, hit.gameObject.transform);
            throwerInstance.GetComponent<StrengthThrower>().Init(enemyLayer, throwerDamage, flyTime, 
                flyForce, Manager.GetDirectionToTarget());
        }
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;
        
        AbilityOffInvoke();
        NextCanUse = Time.time + abilityCooldown;
        isActive = false;
        AbilityOn = false;
    }
}
