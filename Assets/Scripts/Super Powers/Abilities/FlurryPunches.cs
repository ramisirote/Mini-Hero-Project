using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlurryPunches : Ability
{
    [SerializeField] private float damageTotal;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private int numberOfPunches;
    private Transform _armRTransform;
    private Transform _armLTransform;
    private Animator _animator;

    private int _punchesDone;
    
    protected override void AdditionalInit() {
        var effectPoints = parentCharacter.GetComponent<EffectPoints>();
        _armRTransform = effectPoints.GetPointTransform(Refarences.EBodyParts.ArmR);
        _armLTransform = effectPoints.GetPointTransform(Refarences.EBodyParts.ArmL);
        _animator = parentCharacter.GetComponent<Animator>();
    }

    public override void UseAbility(Vector3 direction) {
        if(AbilityOn) return;
        
        Manager.DisableActions();
        _animator.SetBool(AnimRefarences.FlurryPunches, true);
        AbilityOn = true;
        AbilityOnInvoke();
    }

    public override void AnimationTrigger() {
        var armPosition = _punchesDone%2==0 ? _armRTransform.position : _armLTransform.position;
        _punchesDone++;

        var hits = Physics2D.OverlapCircleAll(armPosition, radius, whatIsEnemy);
        foreach (var hit in hits) {
            hit.gameObject.GetComponent<ITakeDamage>().Damage(damageTotal/numberOfPunches, Vector2.zero, true);
        }
        
        if(_punchesDone >= numberOfPunches) SetAbilityOff();
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;
        
        AbilityOn = false;
        _punchesDone = 0;
        Manager.EnableManager();
        _animator.SetBool(AnimRefarences.FlurryPunches, false);
        
        AbilityOffInvoke();
    }
}
