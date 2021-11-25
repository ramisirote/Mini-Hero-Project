using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BurstAbility : Ability
{
    [SerializeField] protected float blastRadius;
    [SerializeField] protected ParticleSystem blastParticle;
    [SerializeField] protected ParticleSystem chargeParticle;
    [SerializeField] protected LayerMask layersToHit;
    [SerializeField] protected float cameraShake;
    
    protected bool powerActive;
    protected Animator _animator;
    protected string animation = AnimRefarences.Burst;
    
    protected override void AdditionalInit() {
        _animator = parentCharacter.GetComponent<Animator>();
        SetUpParticleColors();
        if (blastParticle) blastParticle.transform.localScale *= blastRadius;
    }


    protected abstract void SetUpParticleColors();
    
    public override void AnimationTrigger() {
        if (powerActive) {
            SetAbilityOff();
        }
        else {
            ActivateBlast();
        }
    }
    
    private void ActivateBlast() {
        powerActive = true;
        Controller.StopHorizontal();
        if(chargeParticle) chargeParticle.Stop();
        if(blastParticle) blastParticle.Play();
        
        if(audioSource) audioSource.Play();
        if(IsPlayer) CinemachineShake.Instance.ShakeCamera(cameraShake, 0.5f);
        
        BlastHit();
        
        SetAbilityOff();
    }

    private void BlastHit() {
        var pos = transform.position;

        var hits = Physics2D.OverlapCircleAll(pos, blastRadius, layersToHit);

        foreach (var hit in hits) {
            DoHitEffect(hit);
        }
    }

    protected abstract void DoHitEffect(Collider2D hit);

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn && CharacterStats.UseEnergy(energyRequired)) {
            
            Controller.StopHorizontal();
            _animator.SetTrigger(animation);
            
            if(chargeParticle) chargeParticle.Play();

            AbilityOn = true;
            AdditionalUseAbility();
            AbilityOnInvoke();
        }
        else {
            AbilityOn = false;
        }
    }

    protected abstract void AdditionalUseAbility();

    public override void SetAbilityOff() {
        if(!AbilityOn) return;

        if (chargeParticle) {
            chargeParticle.Stop();
            chargeParticle.Clear();
        }
        AdditionalSetOff();
        powerActive = false;
        AbilityOn = false;
        NextCanUse = Time.time + abilityCooldown * Time.deltaTime;
        
        AbilityOffInvoke();
    }

    protected abstract void AdditionalSetOff();

    protected override void OnDamageTaken(object o, float damageAmount) {
        SetAbilityOff();
    }

    void OnDrawGizmosSelected() {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
