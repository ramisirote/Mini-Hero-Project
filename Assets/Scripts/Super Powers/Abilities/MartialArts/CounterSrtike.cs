using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterSrtike : Ability2
{
    public float hitRadius;
    public LayerMask enemyLayer;
    public float damageMult;
    public float upgradedDamageMult;
    public float increasedDuration;

    private bool _attackBack;
    private bool _increaseDamage;
    private bool _increasedWindow;
    private float damageBlocked;
    private bool didBlock;

    private float resistnace = 1f;
    private bool doingAttack;

    private bool animTriggered;

    private Transform effectPointTransform;

    private float minTime = -1;

    private bool abilityReleased = false;

    protected override void UnlockAbilityMap() {
        _reducedEnergy = upgrades[0];
        _reducedCooldown = upgrades[1];
        _attackBack = true;
        _increaseDamage = upgrades[2];
        _increasedWindow = upgrades[3];

        if(minTime == -1) minTime = windUpTime;
        if(_increasedWindow){
            windUpTime = increasedDuration;
        }
    }

    public override void UseAbility(Vector3 direction)
    {
        Manager.DisableManager();
        Manager.DisableFlip();
        Controller.StopHorizontal();
        didBlock = doingAttack = animTriggered = false;
        CharacterStats.GetCharacterStats().SetAddBuff(CharacterStatsData.StatFields.Resistnce, resistnace);
        damageBlocked = 0f;
        takeDamage.SetPlayAnimatoin(false);
        Animator.SetBool(AnimRefarences.Block, true);
        abilityReleased = false;
        base.UseAbility();
    }

    protected override void AdditionalInit()
    {
        effectPointTransform = effectoints.GetPointTransform(Refarences.EBodyParts.punch);
        minTime = windUpTime;
    }

    protected override bool AditionalWindUp(float currentTimer){
        didBlock = normalizedTimer(Mathf.Min(currentTimer, minTime), minTime) < 0.7f && damageBlocked > 0.1f;
        if (didBlock) return true;
        if (abilityReleased && windUpTime - currentTimer > minTime) return true;
        return false;
    }

    protected override void Activate(){
        if (didBlock){
            StartAttackBack();
            doingAttack = true;
        }
        base.Activate();
    }

    protected override bool AditionalHang(float currentTimer){
        return didAnimTriggerIfAttacked();
    }

    protected override void Deactivate(){
        if(doingAttack) doHit();
        base.Deactivate();
    }

    protected override bool AditionalWindDown(float currentTimer){
        return didAnimTriggerIfAttacked();
    }

    protected override void AditionalAbilityOff(){
        CharacterStats.GetCharacterStats().SetAddBuff(CharacterStatsData.StatFields.Resistnce, resistnace, remove: true);
        Animator.SetBool(AnimRefarences.Block, false);
        takeDamage.SetPlayAnimatoin(true);
    }
    
    protected override void OnDamageTaken(object o, float damageAmount) {
        if(AbilityOn){
            damageBlocked = damageAmount;
        }
    }

    public override void AnimationTrigger()
    {
        animTriggered = true;
    }

    private void StartAttackBack(){
        Animator.SetBool(AnimRefarences.Block, false);
        Animator.SetTrigger(AnimRefarences.StrengthPunch);
        doingAttack = true;
        Manager.FaceTarget(getNearestTarget());
        Manager.DisableManager();
        Manager.DisableFlip();
    }

    private bool didAnimTriggerIfAttacked(){
        if (doingAttack){
            if (animTriggered){
                animTriggered = false;
                return true;
            }
            return false;
        }
        return true;
    }

    private void doHit(){
        var mult = _increaseDamage ? upgradedDamageMult : damageBlocked;
        Collider2D[] hits = Physics2D.OverlapCircleAll(effectPointTransform.position, hitRadius, enemyLayer);
        foreach (var hit in hits) {
            ITakeDamage takeDamage = HitManager.GetTakeDamage(hit.gameObject);
            takeDamage.Damage(damageBlocked * damageMult, Vector2.zero);
        }
        if (hits.Length > 0) {
            if(gameObject.CompareTag("Player")) CinemachineShake.Instance.ShakeCamera();
        }   
    }

    private Transform getNearestTarget(){
        var rayHit = Physics2D.Raycast(effectPointTransform.position, Vector2.right*Controller.GetFacingMult(), 
                                        hitRadius*2.5f, enemyLayer);
        if(rayHit) return rayHit.transform;
        rayHit = Physics2D.Raycast(effectPointTransform.position, Vector2.left*Controller.GetFacingMult(), 
                                        hitRadius*2.5f, enemyLayer);
        if(rayHit) return rayHit.transform;
        return null;
    }

    public override void UseAbilityRelease(Vector3 direction) {
        if(curentState == states.windUp && _increasedWindow){
            abilityReleased = true;
        }
    }
    
}
