using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunStrike : Ability2
{
    public float damage;
    public float hitRadius;
    public LayerMask enemyLayer;
    public float stunTime;
    public float longerStun;
    private bool animTrigger;
    private Transform effectPointTransform;
    private TrailRenderer trailRenderer;
    private bool _longerStun;

    protected override void UnlockAbilityMap() {
        _reducedEnergy = upgrades[0];
        _reducedCooldown = upgrades[1];
        _longerStun = upgrades[2];
        // _dontDisableActions = upgrades[3];
    }
    

    public override void UseAbility(Vector3 direction)
    {
        base.UseAbility();
    }

    protected override void AdditionalInit()
    {
        effectPointTransform = effectoints.GetPointTransform(Refarences.EBodyParts.punch);
        // trailRenderer = effectPointTransform.gameObject.GetComponent<TrailRenderer>();
        
        // trailRenderer.enabled = false;
    }

    public override void AdditionalUseAbility(){
        // start animation
        Vector2 direction = Manager.GetDirectionToTarget();
        direction.Normalize();
        // trailRenderer.enabled = false;
        Animator.SetTrigger(AnimRefarences.StunStrike);
        Manager.DisableManager();
        Manager.DisableFlip();
        Manager.FaceTarget();
        Controller.StopHorizontal();
        animTrigger = false;
    }

    protected override bool AditionalWindUp(float currentTimer){
        return animTrigger;
    }



    public override void AnimationTrigger() {
        if(curentState == states.windUp){
            // trailRenderer.Clear();
            // trailRenderer.enabled = true;
            animTrigger = true;
        }
        else{
            SetAbilityOff();
        }
    }

    protected override void Activate(){
        base.Activate();
        DoHit();
    }

    private void DoHit(){
        Collider2D[] hits = Physics2D.OverlapCircleAll(effectPointTransform.position, hitRadius, enemyLayer);
        foreach (var hit in hits) {
            ITakeDamage takeDamage = HitManager.GetTakeDamage(hit.gameObject);
            takeDamage.Damage(damage, Vector2.zero);
            if(_longerStun) takeDamage.Stun(longerStun); else takeDamage.Stun(stunTime);
        }
        if (hits.Length > 0) {
            if(gameObject.CompareTag("Player")) CinemachineShake.Instance.ShakeCamera();
        }
    }

    protected override void AditionalAbilityOff(){
        // trailRenderer.enabled = false;
        // trailRenderer.Clear();
    }
}
