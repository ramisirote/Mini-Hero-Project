using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthSmash : Ability
{
    [SerializeField] private Vector2 pushSelfForce;
    [SerializeField] private Vector2 pushOthersForce;
    [SerializeField] private float damage;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private ParticleSystem dustParticles;

    [SerializeField] private float cameraShake;
    [SerializeField] private float shakeTime;

    private bool _wasFlying;
    private Animator _animator;
    private bool _abilityActive;
    private TakeDamage _parentTakeDamage;
    private Transform impactPoint;
    
    protected override void AdditionalInit() {
        _animator = parentCharacter.GetComponent<Animator>();
        transform.position = parentCharacter.transform.position;
        _parentTakeDamage = parentCharacter.GetComponent<TakeDamage>();
        impactPoint = parentCharacter.GetComponent<EffectPoints>().GetPointTransform(Refarences.EBodyParts.ArmR);
        dustParticles.Stop();
    }

    private void Update() {
        if (AbilityOn && Controller.IsGrounded()) {
            Controller.StopHorizontal();
        }
    }

    public override void UseAbility(Vector3 direction) {
        if(AbilityOn) return;

        AbilityOn = true;

        _wasFlying = Controller.IsFlying();
        Manager.Fly(false);
        _parentTakeDamage.SetPlayAnimatoin(false);
        Manager.DisableManager();
        if (!Controller.IsGrounded()) {
            Controller.Push(pushSelfForce*Controller.GetFacingMult());
        }
        else {
            Controller.StopHorizontal();
        }
        _animator.SetTrigger(AnimRefarences.Smash);
        AbilityOnInvoke();
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;
        
        AbilityOn = false;
        _abilityActive = false;
        AbilityOffInvoke();
        
        _parentTakeDamage.SetPlayAnimatoin(true);
        
        // Animation enables the manager
    }

    private void DoGroundImpact() {
        Controller.StopAll();

        var impactPosition = impactPoint.position;
        dustParticles.transform.position = impactPosition;
        dustParticles.Play();
        
        if(IsPlayer) CinemachineShake.Instance.ShakeCamera(cameraShake, shakeTime);
        
        
        var position = transform.position;
        var hitDetect = Physics2D.OverlapCircleAll(impactPosition, radius, enemyLayerMask);
        
        foreach (var hit in hitDetect) {
            var damager = HitManager.GetTakeDamage(hit.gameObject);
            Vector2 directionMultVec = Vector2.one;
            var hitPosition = hit.transform.position;
            if (hitPosition.x < position.x) { directionMultVec.x = -1; }
            damager?.Damage(damage, pushOthersForce*directionMultVec);
        }
    }

    public override void AnimationTrigger() {
        if (!_abilityActive) {
            DoGroundImpact();
            _abilityActive = true;
        }
        else {
            SetAbilityOff();
        }
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
