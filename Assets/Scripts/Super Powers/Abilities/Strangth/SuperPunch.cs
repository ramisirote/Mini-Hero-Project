using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperPunch : Ability
{
    [SerializeField] private float baseDamage;
    [SerializeField] private float damageHoldInc;
    [SerializeField] private float knockBack;
    [SerializeField] private float hitRadius;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float chargePerSecond;
    [SerializeField] private float maxCharge;

    private Animator _animator;
    private bool _released;
    private Coroutine _coroutine;
    private Transform effectPointTransform;
    private Transform[] _arms = new Transform[2];
    private BodyAngler _bodyAngler;
    private float[] _armDefaultRotationAngles = new float[]{-1,-1};
    private bool _didHitTrigger;
    private float _changedAmount;
    
    // Upgrades
    [Header("Upgrades")] 
    [SerializeField] private float reducedEnergy;
    [SerializeField] private float reducedCooldown;
    [SerializeField] private float increasedKnockback;
    [SerializeField] private float shortOnTime;
    private bool _reducedEnergy;
    private bool _reducedCooldown;
    private bool _increasedKnockback;
    private bool _isHold = true;
    
    protected override void UnlockAbilityMap() {
        _reducedEnergy = upgrades[0];
        _reducedCooldown = upgrades[1];
        _increasedKnockback = upgrades[2];
        // _isHold = upgrades[3];
        
        abilityCooldown = _reducedCooldown ? reducedCooldown : abilityCooldown;
        energyRequired = _reducedEnergy ? reducedEnergy : energyRequired;
    }

    protected override void AdditionalInit() {
        _animator = Animator;
        _bodyAngler = parentCharacter.GetComponent<BodyAngler>();
        SetUpArmAngler();
    }
    
    private void SetUpArmAngler() {
        var ep = parentCharacter.GetComponent<EffectPoints>();
        effectPointTransform = ep.GetPointTransform(Refarences.EBodyParts.punch);
        var upperArmR = ep.GetJointObject(Refarences.BodyJoints.ArmRUpper);
        var upperArmL = ep.GetJointObject(Refarences.BodyJoints.ArmLUpper);
        if (upperArmR && upperArmL) {
            _arms[0] = upperArmR.transform.parent;
            _arms[1] = upperArmL.transform.parent;
            for (var i=0; i<2; i++) _armDefaultRotationAngles[i] = _arms[i].localRotation.eulerAngles.z;
        }
        
        
        _bodyAngler = parentCharacter.GetComponent<BodyAngler>();
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn && CharacterStats.UseEnergy(energyRequired)) {
            Manager.DisableManager();
            Manager.DisableFlip();
            AbilityOn = true;
            _released = false;
            _didHitTrigger = false;
            _changedAmount = 0;
            _animator.SetBool(AnimRefarences.PunchLoad, true);
            
            NextCanUse = Time.time + abilityCooldown * Time.deltaTime;
            if (!_isHold) {
                Release();
            }
            AbilityOnInvoke();
        }
    }

    private void Release() {
        _released = true;
        _animator.SetBool(AnimRefarences.PunchLoad, false);
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;

        if (_bodyAngler && _arms[0] && _arms[1]) {
            for(int i=0; i<2; i++) _bodyAngler.RotatePart(_arms[i], _armDefaultRotationAngles[i]);
        }

        NextCanUse = Time.time + abilityCooldown;
        
        Manager.EnableManager();
        
        AbilityOffInvoke();

        AbilityOn = false;
    }

    public override void UseAbilityRelease(Vector3 direction) {
        if(AbilityOn && !_released) Release(); 
    }

    public override void AnimationTrigger() {
        if (!_didHitTrigger) {
            _didHitTrigger = true;
            DoHit();
        }
        else {
            SetAbilityOff();
        }
    }

    public void DoHit() {
        Controller.StopHorizontal();
        Collider2D[] hits = Physics2D.OverlapCircleAll(effectPointTransform.position, hitRadius, enemyLayer);
        foreach (var hit in hits) {
            HitManager.GetTakeDamage(hit.gameObject).Damage(baseDamage+damageHoldInc*_changedAmount,
                effectPointTransform.position, knockBack);
        }
        if (hits.Length > 0) {
            Controller.StopHorizontal();
            if(gameObject.CompareTag("Player")) CinemachineShake.Instance.ShakeCamera();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (AbilityOn && !_released) {
            _changedAmount = Mathf.Min(_changedAmount + chargePerSecond * Time.deltaTime, maxCharge);
            Controller.StopHorizontal();
        }
    }
    
    public override void UpdateDirection(Vector3 direction) {
        if(!AbilityOn || _released) return;
        
        float angle;
        Manager.FaceTarget();
        angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90 )* -Controller.GetFacingMult();

        
        if (_bodyAngler && _arms[0] && _arms[1]) {
            //_bodyAngler.RotatePart(_arms[0], angle);
            _bodyAngler.RotatePart(_arms[1], angle);
        }
    }
}
