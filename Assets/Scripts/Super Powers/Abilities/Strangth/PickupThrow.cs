using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;

public class PickupThrow : Ability
{
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private float cameraShake;
    [SerializeField] private float grabRadius;
    [SerializeField] private GameObject thrower;
    [SerializeField] private float rotationOffset;
    [SerializeField] private float throwSpeed;

    [Header("Thrower Properties")] 
    [SerializeField] private float throwerDamage;
    [SerializeField] private float flyTime;
    [SerializeField] private float flyForce;
    [SerializeField] private LayerMask hitLayer;

    private Animator _animator;
    private Coroutine _coroutine;
    private Transform effectPointTransform;
    private Transform[] _arms;
    private float[] _armDefaultRotationAngles;
    private BodyAngler _bodyAngler;

    private GameObject _grabed;
    private Quaternion _grabedDefaultRotation;
    private IManager _grabedManager;
    private float _grabbedRotationMult;
    private Vector3 _grabbedFacing;

    private bool _released = false;
    private bool _waitingForAnimEnd;
    private TakeDamage _parentTakeDamage;
    
    //Unlocks
    [Header("Upgrades")] 
    [SerializeField] private float reducedEnergy;
    [SerializeField] private float reducedCooldown;
    private bool _reducedEnergy;
    private bool _reducedCooldown;
    private bool _hitMultiple;
    private bool _canHold;

    protected override void UnlockAbilityMap() {
        _reducedEnergy = upgrades[0];
        _reducedCooldown = upgrades[1];
        _hitMultiple = upgrades[2];
        _canHold = upgrades[3];
        
        abilityCooldown = _reducedCooldown ? reducedCooldown : abilityCooldown;
        energyRequired = _reducedEnergy ? reducedEnergy : energyRequired;
    }
    
    protected override void OnDamageTaken(object o, float damageAmount) {
        if (!AbilityOn) return; 
        StartCoroutine(Throw());
    }
    
    protected override void AdditionalInit() {
        _animator = parentCharacter.GetComponent<Animator>();
        var t = transform;
        _parentTakeDamage = parentCharacter.GetComponent<TakeDamage>();
        SetUpArmAngler();
    }
    
    void SetUpArmAngler() {
        var ep = parentCharacter.GetComponent<EffectPoints>();
        effectPointTransform = ep.GetPointTransform(Refarences.EBodyParts.ArmL);
        var upperArmRight = ep.GetJointObject(Refarences.BodyJoints.ArmRUpper);
        var upperArmLeft = ep.GetJointObject(Refarences.BodyJoints.ArmLUpper);
        _arms = new Transform[2];
        if (upperArmRight) {
            _arms[0] = upperArmRight.transform.parent;
        }

        if (upperArmLeft) {
            _arms[1] = upperArmLeft.transform.parent;
        }

        _armDefaultRotationAngles = new float[2];
        for (var i=0; i<2; i++) {
            if (_arms[i]) _armDefaultRotationAngles[i] = _arms[i].localRotation.eulerAngles.z;
        }
        
        _bodyAngler = parentCharacter.GetComponent<BodyAngler>();
        // transform.parent = effectPointTransform;
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn && CharacterStats.UseEnergy(energyRequired)) {
            AbilityOn = true;
            _animator.SetTrigger(AnimRefarences.Grab);
            Manager.FaceTarget();
            Manager.DisableManager();
            
            _parentTakeDamage.SetPlayAnimatoin(false);
            Controller.StopHorizontal();
            _released = false;
            _waitingForAnimEnd = false;
            AbilityOnInvoke();
        }
        else {
            SetAbilityOff();
        }
    }
    
    public override void AnimationTrigger() {
        if(!AbilityOn) return;
        if (!_waitingForAnimEnd) {
            _waitingForAnimEnd = true;
            PickUp();
        }
        else {
            SetAbilityOff();
        }
    }

    private void PickUp() {
        var hit = Physics2D.OverlapCircle(effectPointTransform.position, grabRadius, pickupLayer);
        if (!hit) return;
        _animator.SetBool(AnimRefarences.HandsOut, true);
        for (var i = 0; i < 2; i++) _bodyAngler.RotatePart(_arms[i], 180);
        Manager.EnableMove();
        _grabed = hit.gameObject;
        _grabedDefaultRotation = _grabed.transform.rotation;
        _grabedManager = _grabed.GetComponent<IManager>();
        _grabedManager?.Stunned();

        _grabbedFacing = _grabed.transform.localScale;
        _grabbedRotationMult = Controller.GetFacingMult() * _grabbedFacing.x > 0 ? 1:-1;

        if (_released || !_canHold) StartCoroutine(Throw());
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;

        AbilityOn = false;
        
        if (_bodyAngler && _arms[0]) {
            _bodyAngler.RotatePart(_arms[0], _armDefaultRotationAngles[0]);
            _bodyAngler.RotatePart(_arms[1], _armDefaultRotationAngles[1]);
        }
        
        _animator.SetBool(AnimRefarences.HandsOut, false);
        

        if (_grabed) {
            _grabedManager?.Stunned(false);
            _grabed.transform.rotation = _grabedDefaultRotation;
            _grabed.transform.localScale = _grabbedFacing;
        }
        _grabed = null;
        _grabedManager = null;
        
        Manager.EnableManager();
        _parentTakeDamage.SetPlayAnimatoin(true);

        NextCanUse = Time.time + abilityCooldown;
        AbilityOffInvoke();
    }
    
    public override void UseAbilityRelease(Vector3 direction) {
        if (_grabed) {
            StartCoroutine(Throw());
        }
        
        _released = true;
    }

    private IEnumerator Throw() {
        var direction = Manager.GetDirectionToTarget();
        Manager.FaceTarget();
        Manager.DisableMove();
        Manager.DisableFlip();
        float throwTimer = throwSpeed/2;
        float targetAngle;
        while (throwTimer > 0) {
            targetAngle = Mathf.Lerp(180, 130, 1-throwTimer/(throwSpeed/2));
            for (var i = 0; i < 2; i++) _bodyAngler.RotatePart(_arms[i], targetAngle);
            
            throwTimer -= Time.deltaTime;
            yield return null;
        }
        throwTimer = throwSpeed/2;
        while (throwTimer > 0) {
            targetAngle = Mathf.Lerp(130, 270, 1-throwTimer/(throwSpeed/2));
            for (var i = 0; i < 2; i++) _bodyAngler.RotatePart(_arms[i], targetAngle);
            if (throwTimer < throwSpeed/4) {
                ThrowRelease(direction);
            }
            throwTimer -= Time.deltaTime;
            yield return null;
        }
        SetAbilityOff();
    }

    private void ThrowRelease(Vector3 direction) {
        if (_grabed) {
            _grabed.transform.rotation = _grabedDefaultRotation;
            _grabed.transform.localScale = _grabbedFacing;

            var throwerInstance = Instantiate(thrower, _grabed.gameObject.transform);
            throwerInstance.GetComponent<StrengthThrower>().Init(hitLayer, throwerDamage, flyTime, 
                flyForce,direction, _hitMultiple);
            _grabed = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_grabed) {
            SetPickUpAngle();
            if (_grabed.activeSelf == false) {
                SetAbilityOff();
            }
        }
        else if (_grabedManager != null) {
            SetAbilityOff();
        }
    }

    private void SetPickUpAngle() {
        _grabed.transform.position = effectPointTransform.position;
        var facing = Controller.GetFacingMult();

        _grabed.transform.rotation =  Quaternion.Euler(new Vector3(0,0,_grabbedRotationMult*90));
        var grabedScaleAbs = _grabed.transform.localScale.Abs();
        _grabed.transform.localScale = new Vector3(grabedScaleAbs.x, _grabbedRotationMult*facing*grabedScaleAbs.y, grabedScaleAbs.z);
    }
}
