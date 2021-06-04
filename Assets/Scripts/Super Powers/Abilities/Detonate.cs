using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detonate : Ability
{
    [SerializeField] private float timer;
    [SerializeField] private float damage;
    [SerializeField] private float detonateRadius;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float hitRadius;
    [SerializeField] private GameObject detonator;
    [SerializeField] private float knockBack;
    
    private Transform _handTransform;
    private Transform[] _arms;
    private float[] _armDefaultRotationAngles;
    private BodyAngler _bodyAngler;
    
    private bool isActive = false;
    private bool _foundTarget;
    
    // Upgrades
    [Header("Upgrades")] 
    [SerializeField] private float reducedEnergy;
    [SerializeField] private float reducedCooldown;
    [SerializeField] private float reducedTimer;
    private bool _reducedEnergy;
    private bool _reducedCooldown;
    private bool _canChargeGround;
    private bool _reduceTimer;

    protected override void UnlockAbilityMap() {
        _reducedEnergy = upgrades[0];
        _reducedCooldown = upgrades[1];
        _canChargeGround = upgrades[2];
        _reduceTimer = upgrades[3];

        timer = _reduceTimer ? reducedTimer : timer;
        abilityCooldown = _reducedCooldown ? reducedCooldown : abilityCooldown;
        energyRequired = _reducedEnergy ? reducedEnergy : energyRequired;
    }
    
    protected override void OnDamageTaken(object o, float damageAmount) {
        if(AbilityOn) SetAbilityOff();
    }

    protected override void AdditionalInit() {
        _handTransform = parentCharacter.GetComponent<EffectPoints>().GetPointTransform(Refarences.EBodyParts.ArmR);
        SetUpArmAngler();
    }
    
    
    
    void SetUpArmAngler() {
        var ep = parentCharacter.GetComponent<EffectPoints>();
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
    }

    public override void UseAbility(Vector3 direction) {
        if(AbilityOn || !CharacterStats.HasRequiredEnergy(energyRequired)) return;
        _foundTarget = false;
        Animator.SetTrigger(AnimRefarences.Grab);
        RotateArms();
        Manager.DisableManager();
        Controller.StopHorizontal();
        Manager.FaceTarget();
        AbilityOnInvoke();
        isActive = false;
        AbilityOn = true;
    }

    private void RotateArms() {
        float angle;
        Manager.FaceTarget();
        var direction = Manager.GetDirectionToTarget();
        angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90 )* -Controller.GetFacingMult();
        
        if (_bodyAngler) {
            for (var i = 0; i < 2; i++) {
                _bodyAngler.RotatePart(_arms[i], angle);
            }
        }
    }


    public override void AnimationTrigger() {
        if(!AbilityOn) return;
        
        if (!isActive) {
            MakeDetonator();
            isActive = true;
        }
        else {
            SetAbilityOff();
        }
    }

    private void MakeDetonator() {
        Controller.StopHorizontal();
        var hit = Physics2D.OverlapCircle(_handTransform.position, hitRadius, enemyLayer);
        if (hit && !hit.GetComponent<CharacterStats>().IsDead() && CharacterStats.UseEnergy(energyRequired)) {
            _foundTarget = true;
            hit.GetComponent<IManager>().DisableManager();
            var detonatorInstance = Instantiate(detonator, hit.gameObject.transform);
            detonatorInstance.GetComponent<Detonator>().Init(enemyLayer, damage, timer, detonateRadius, knockBack, Colors);
            return;
        }
        if(!_canChargeGround) return;
        var rayHit = Physics2D.CircleCast(_handTransform.position, hitRadius, Vector2.right, 0, groundLayer);
        if (rayHit && CharacterStats.UseEnergy(energyRequired)) {
            _foundTarget = true;
            var detonatorInstance = Instantiate(detonator, rayHit.collider.transform);
            detonatorInstance.transform.position = rayHit.point;
            detonatorInstance.GetComponent<Detonator>().Init(enemyLayer, damage, timer, detonateRadius, knockBack, Colors);
            return;
        }
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;
        
        if (_foundTarget) NextCanUse = Time.time + abilityCooldown;
        AbilityOffInvoke();
        if (_bodyAngler) {
            for(var i=0; i<2; i++) _bodyAngler.RotatePart(_arms[i], _armDefaultRotationAngles[i]);
        }
        isActive = false;
        AbilityOn = false;
        Manager.EnableManager();
    }
}
