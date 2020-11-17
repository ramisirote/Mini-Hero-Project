using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Charge a bolt that gets bigger and deals more damage the more its charged
 * Part of the Energy Power class.
 * Strengths: Range, high damage if fully charged, no disable.
 * Weaknesses: High energy cost up front.
 */
public class EnergyBoltCharge : Ability
{

    [SerializeField] private float damagePerCharge;
    [SerializeField] private ParticleSystem chargeParticle;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float chargePerSecond;
    [SerializeField] private float maxCharge;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private LayerMask enemyLayers;
    
    private Transform effectPointTransform;
    private Animator _animator;
    private ParticleSystem.MainModule _particleSystemMain;
    
    private Transform _arm;
    private float _armDefaultRotationAngle = -1;
    private BodyAngler _bodyAngler;

    private float _chargeAmount = 0;

    private EnergyChargeProjectile _chargeProjectile = null;

    private GameObject _curentChaging;

    protected override void OnDamageTaken(object o, float damageAmount) {
        if(AbilityOn && _curentChaging) _animator.SetBool(AnimRefarences.IsFireingContinues, false);
        SetAbilityOff();
    }

    protected override void AdditionalInit() {
        SetUpParticleSystem();
        SetUpArmAngler();
        _animator = parentCharacter.GetComponent<Animator>();
        
    }
    
    private void Update() {

        if (AbilityOn && _chargeProjectile) {
            _chargeProjectile.AddCharge(chargePerSecond * Time.deltaTime);
            if (IsPlayer) {
                CinemachineShake.Instance.ShakeCamera(0.2f*_chargeProjectile.GetChargeNormalized());
            }

        }
        
        // // move to arm position
        if (effectPointTransform) {
            transform.position = effectPointTransform.position;
            if (_curentChaging) {
                _curentChaging.transform.position = transform.position;
            }
        }
    }
    
    
    private void SetUpParticleSystem() {
        var col = chargeParticle.colorOverLifetime;
        col.color = Utils.CreateGradient(new []{_color1, _color2, _color3},new []{0f, 0.5f,0.8f});
    }
    
    
    private void SetUpArmAngler() {
        var ep = parentCharacter.GetComponent<EffectPoints>();
        effectPointTransform = ep.GetPointTransform(Refarences.EBodyParts.ArmR);
        var upperArm = ep.GetJointObject(Refarences.BodyJoints.ArmRUpper);
        if (upperArm) {
            _arm = upperArm.transform.parent;
        }
        
        _bodyAngler = parentCharacter.GetComponent<BodyAngler>();
        transform.parent = effectPointTransform;
        transform.position = effectPointTransform.position;
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn && CharacterStats.UseEnergy(energyRequired)) {
            Manager.DisableActions();
            Manager.DisableFlip();
            AbilityOn = true;
            _animator.SetBool(AnimRefarences.IsFireingContinues, true);    
            chargeParticle.Play();
            CreateProjectile();
            if (_arm && _armDefaultRotationAngle == -1) {
                _armDefaultRotationAngle = _arm.localRotation.eulerAngles.z;
            }
            
            NextCanUse = Time.time + abilityCooldown * Time.deltaTime;
            AbilityOnInvoke();
        }
    }

    private void CreateProjectile() {
        _curentChaging = Instantiate(projectile, transform);
        _curentChaging.transform.position = effectPointTransform.position;
        _chargeProjectile = _curentChaging.GetComponent<EnergyChargeProjectile>();
        _chargeProjectile.SetUp(maxCharge, enemyLayers, damagePerCharge, _color1, _color2, _color3,
            IsPlayer);
    }
    
    public override void UseAbilityRelease(Vector3 direction) {
        if(AbilityOn) SetPowerOff(direction);
    }

    public void SetPowerOff(Vector3 direction) {
        if(!AbilityOn) return;
        
        _animator.SetBool(AnimRefarences.IsFireingContinues, false);
        
        if (_curentChaging && _chargeProjectile) {
            _curentChaging.transform.parent = null;
            _curentChaging = null;
            _chargeProjectile.Releace(direction, projectileSpeed);
            _chargeProjectile = null;
        }
        
        if (chargeParticle) {
            chargeParticle.Stop();
            chargeParticle.Clear();
        }
        
        if(audioSource) audioSource.Stop();

        if (_bodyAngler && _arm && !CharacterStats.IsDead()) {
            _bodyAngler.RotatePart(_arm, _armDefaultRotationAngle);
        }
        
        Manager.EnableManager();
        
        AbilityOffInvoke();
        
        
        
        AbilityOn = false;
    }

    public override void SetAbilityOff() {
        CancelCharge();
        SetPowerOff(Vector3.zero);
    }

    private void CancelCharge() {
        Destroy(_curentChaging);
        _curentChaging = null;
        _chargeProjectile = null;
    }
    
    public override void UpdateDirection(Vector3 direction) {
        if(!AbilityOn) return;
        
        float angle;
        Manager.FaceTarget();
        angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90 )* -Controller.GetFacingMult();

        
        if (_bodyAngler && _arm) {
            _bodyAngler.RotatePart(_arm, angle);
        }
    }
}
