using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/*
 * A charged beam attack. Charge for a bit, then fires for a bit. Stops at the first enemy.
 * Part of the Energy Power class.
 * Strengths: Range, low cooldown, easy to use and aim, high damage if hit or hit multiple targets.
 * Weaknesses: high energy cost per-damage, disable while on, charge time.
 */
public class EnergyBeam : Ability
{
    [SerializeField] private float damage;
    [SerializeField] private ParticleSystem chargingParticles;
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float OnForTime;
    [SerializeField] private float chargeTime;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float cameraShake;

    private Animator _animator;
    private Coroutine _coroutine;
    private Transform effectPointTransform;
    private Transform _arm;
    private float _armDefaultRotationAngle;
    private BodyAngler _bodyAngler;
    private ParticleSystem hitParticlesInstance;
    private bool _isActive;
    private bool _released;
    private Vector3 _setTarget;
    private float _minChangeTime = 0.1f;
    
    // Upgrades
    [Header("Upgrades")] 
    [SerializeField] private float reducedEnergy;
    [SerializeField] private float reducedCooldown;
    [SerializeField] private float increasedKnockback;
    [SerializeField] private float shortOnTime;
    private bool _reducedEnergy;
    private bool _reducedCooldown;
    private bool _increasedKnockback;
    private bool _isHold;
    
    protected override void UnlockAbilityMap() {
        _reducedEnergy = upgrades[0];
        _reducedCooldown = upgrades[1];
        _increasedKnockback = upgrades[2];
        _isHold = upgrades[3];
        
        abilityCooldown = _reducedCooldown ? reducedCooldown : abilityCooldown;
        energyRequired = _reducedEnergy ? reducedEnergy : energyRequired;
        //OnForTime = !_isHold ? shortOnTime : OnForTime;
    }

    
    protected override void AdditionalInit() {
        lineRenderer.colorGradient = Utils.CreateGradient(new[] {_color3,_color2, _color1}, new[] {0.8f});
        lineRenderer.enabled = false;
        _animator = parentCharacter.GetComponent<Animator>();
        var t = transform;
        SetUpArmAngler();
        SetUpParticles();
    }

    private void SetUpParticles() {
        // Charging particles
        var col = chargingParticles.colorOverLifetime;
        col.color = Utils.CreateGradient(new[] {_color1, _color2, _color3}, new[] {0.5f, 0.8f});
        
        // hit particles
        hitParticlesInstance = Instantiate(hitParticles, parentCharacter.transform);
        var hitParticlesMain = hitParticlesInstance.main;
        var minMaxGradient = hitParticlesMain.startColor;
        minMaxGradient.color = new Color(_color1.r, _color1.g, _color1.b, 0.3f);
        hitParticlesMain.startColor = minMaxGradient;
        hitParticlesInstance.Stop();
    }
    
    void SetUpArmAngler() {
        var ep = parentCharacter.GetComponent<EffectPoints>();
        effectPointTransform = ep.GetPointTransform(Refarences.EBodyParts.ArmR);
        var upperArm = ep.GetJointObject(Refarences.BodyJoints.ArmRUpper);
        if (upperArm) {
            _arm = upperArm.transform.parent;
        }
        if (_arm) _armDefaultRotationAngle = _arm.localRotation.eulerAngles.z;
        _bodyAngler = parentCharacter.GetComponent<BodyAngler>();
        transform.parent = effectPointTransform;
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn) {
            AbilityOn = true;
            _isActive = false;
            _released = false;
            chargingParticles.Play();
            _animator.SetBool(AnimRefarences.IsFireingContinues, true);
            _coroutine = StartCoroutine(DelayOn());
            Manager.DisableMove();
            Manager.DisableActions();
            Controller.StopHorizontal();
            if (!_isHold) {
                UseAbilityRelease(Manager.GetDirectionToTarget());
            }
            AbilityOnInvoke();
        }
        else {
            StartCoroutine(DelayOff());
        }
    }

    IEnumerator DelayOn() {
        var timer = chargeTime;
        while (true) {
            if (timer <= 0) break;
            if (_released && timer <= chargeTime - _minChangeTime) break;
            if (_isActive) break;
            yield return null;
            timer -= Time.deltaTime;
        }
        
        Activate();
    }

    private void Activate() {
        if (_isActive) return;
        _isActive = true;
        Controller.StopHorizontal();
        lineRenderer.enabled = true;
        HandleLineHit();
        _coroutine = StartCoroutine(DelayOff());
    }

    private void Update() {
        if (AbilityOn) {
            Controller.StopHorizontal();
            var position = effectPointTransform.position;
            chargingParticles.transform.position = position;
            HandleLineHit();
            if (parentCharacter.CompareTag("Player")) CinemachineShake.Instance.ShakeCamera(cameraShake);
        }
    }

    private void HandleLineHit() {
        var position = effectPointTransform.position;
        if (lineRenderer.enabled) {
            // set the start of the line to the effect point
            lineRenderer.SetPosition(0, position);

            var target = _released ? _setTarget : Manager.GetDirectionToTarget();
            RaycastHit2D rayHit = Physics2D.Raycast(transform.position,target , 100, layerMask);
            if (rayHit.collider) {
                lineRenderer.SetPosition(1, rayHit.point);
                var enemyHit = HitManager.GetTakeDamage(rayHit.collider.gameObject);
                var knockBack = _increasedKnockback ? increasedKnockback : 0;
                enemyHit?.Damage(damage, effectPointTransform.position, knockBack);

                hitParticlesInstance.transform.position = rayHit.point;
                hitParticlesInstance.Play();
            }
            else {
                Vector2 endTarget = transform.position + parentCharacter.transform.right * 100f;
                lineRenderer.SetPosition(1, endTarget);
            }
        }
    }

    IEnumerator DelayOff() {
        var onTimer = _isHold && !_released ? OnForTime : shortOnTime;
        yield return new WaitForSeconds(onTimer);
        
        SetAbilityOff();
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;

        AbilityOn = false;
        
        if (_bodyAngler && _arm) {
            _bodyAngler.RotatePart(_arm, _armDefaultRotationAngle);
        }
        
        chargingParticles.Stop();
        hitParticlesInstance.Stop();
        hitParticlesInstance.Clear();
        
        StopCoroutine(_coroutine);
        _coroutine = null;

        lineRenderer.enabled = false;
        _animator.SetBool(AnimRefarences.IsFireingContinues, false);
        
        Manager.EnableManager();
        AbilityOffInvoke();
    }
    
    public override void UseAbilityRelease(Vector3 direction) {
        if (!AbilityOn) return;
        if (_isActive) {
            SetAbilityOff();
        }
        else {
            _released = true;
            _setTarget = direction;
            Manager.FaceTarget();
        }
    }
    
    public override void UpdateDirection(Vector3 direction) {
        if (_released) direction = _setTarget;
        else Manager.FaceTarget();
        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90 )* -Controller.GetFacingMult();

        // var transformLocalRotation = transform.localRotation;
        // transformLocalRotation.eulerAngles = new Vector3(0,0,angle);
        
        if (_bodyAngler && _arm) {
            _bodyAngler.RotatePart(_arm, angle);
        }
    }

    protected override void OnDamageTaken(object o, float damageAmount) {
        SetAbilityOff();
    }
}
