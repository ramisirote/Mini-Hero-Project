using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyProjection : Ability
{
    [SerializeField] private LineRenderer energyLine;
    [SerializeField] private float lineVisibleFor;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float damage;
    [SerializeField] private ParticleSystem powEffect;
    private float _turnLineInvisibleTime = 0;
    private Animator _animator;
    private float _phaseTime;
    private float _solidPhaseTime;
    private ParticleSystem _startPowEffect;
    private ParticleSystem _endPowEffect;
    private Vector2 _direction;
    private EffectPoints _effectPoints;
    private BodyAngler _bodyAngler;


    private Transform _arm = null;
    private Vector3 upperArmRotationDefault;

    private bool powerActive;

    protected override void AdditionalInit() {
        if (energyLine) {
            energyLine.colorGradient = LineColorGradient();
            energyLine.enabled = false;
        }

        _animator = parentCharacter.GetComponent<Animator>();
        
        _effectPoints = parentCharacter.GetComponent<EffectPoints>();
        if (_effectPoints != null) {
            Transform armEffect = _effectPoints.GetPointTransform(Refarences.EBodyParts.ArmR);
            if (armEffect == null) {
                transform.position = parentCharacter.transform.position;
            }
            else {
                Transform myTransform = transform;
                myTransform.parent = armEffect;
                myTransform.position = armEffect.position;
            }
            
            var upperArmTransform = _effectPoints.GetJointObject(Refarences.BodyJoints.ArmRUpper);
            if (upperArmTransform) {
                _arm = upperArmTransform.transform.parent;
                if (_arm) {
                    upperArmRotationDefault = _arm.eulerAngles;
                    _bodyAngler = parentCharacter.GetComponent<BodyAngler>();
                }
            }

        }
        else {
            transform.position = parentCharacter.transform.position;
        }
        
        ParticleInstant();
    }

    protected override void OnDamageTaken(object o, float damageAmount) {
        if (AbilityOn) {
            SetAbilityOff();
        }
    }

    private Gradient LineColorGradient() {
        var gradient = new Gradient();
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = _color1;
        colorKey[0].time = 0.0f;
        colorKey[1].color = _color2;
        colorKey[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 0.7f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.7f;
        alphaKey[1].time = 1.0f;
        
        gradient.SetKeys(colorKey, alphaKey);
        return gradient;
    }

    private void FixedUpdate() {
        if(energyLine.enabled) {
            energyLine.SetPosition(0, transform.position);
        }

        // if (PowerOn) {
        //     Manager.FacePowerTarget();
        // }
    }

    private void ParticleInstant() {
        _startPowEffect = Instantiate(powEffect, transform);
        var mainModule = _startPowEffect.main;
        mainModule.startColor = new Color(_color1.r, _color1.g, _color1.b, 0.7f);
        _startPowEffect.transform.position = transform.position;
        _endPowEffect = Instantiate(powEffect);
        mainModule = _endPowEffect.main;
        mainModule.startColor = new Color(_color2.r, _color2.g, _color2.b, 0.7f);
        _startPowEffect.Stop();
        _endPowEffect.Stop();
    }

    public override void AnimationTrigger() {
        if (!energyLine) return;

        if (powerActive) {
            SetAbilityOff();
        }
        else {
            Controller.StopHorizontal();
            ActivateBlast();
        }
    }


    private void ActivateBlast() {
        powerActive = true;
        Controller.StopHorizontal();
        if (!_bodyAngler && _arm) {
            _arm.eulerAngles = new Vector3(0,0,45*Controller.GetFacingMult());
        }
        
        energyLine.enabled = true;
        energyLine.SetPosition(0, transform.position);
        if(audioSource) audioSource.Play();
        int facingDirectionMult = 1;
        if (parentCharacter.transform.localScale.x > 0) {
            facingDirectionMult = -1;
        }

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, _direction, 100, enemyLayer);
        if (rayHit.collider) {
            energyLine.SetPosition(1, rayHit.point);
            TakeDamage enemyHit = rayHit.collider.GetComponent<TakeDamage>();
            if (enemyHit) {
                enemyHit.Damage(damage, facingDirectionMult);
            }

            _endPowEffect.transform.position = rayHit.point;
            _endPowEffect.Play();
        }
        else {
            Vector2 endTarget = transform.position + parentCharacter.transform.right * 100f;
            energyLine.SetPosition(1, endTarget);
        }
        energyLine.enabled = true;
        StartCoroutine(MakeLineInvisibleFor(lineVisibleFor));
        // _turnLineInvisibleTime = Time.time + Time.deltaTime * lineVisibleFor;
        NextCanUse = Time.time + abilityCooldown * Time.deltaTime;
        
        _startPowEffect.Play();
    }

    IEnumerator MakeLineInvisibleFor(float seconds) {
        yield return new WaitForSeconds(seconds);
        if (powerActive && energyLine) {
            energyLine.enabled = false;
        }
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;
        
        powerActive = false;
        
        if (energyLine) {
            energyLine.enabled = false;
        }
        
        AbilityOn = false;

        if (_arm) {
            if (_bodyAngler) {
                _bodyAngler.RotatePart(_arm, upperArmRotationDefault);
            }
            else {
                _arm.eulerAngles = upperArmRotationDefault;
            }
        }
        AbilityOffInvoke();
    }

    public override void UseAbility(Vector3 vectorToTarget) {
        if (!AbilityOn && CharacterStats.UseEnergy(energyRequired)) {
            Manager.DisableFlip();
            Manager.FacePowerTarget();
            Controller.StopHorizontal();
            _direction = vectorToTarget;
            
            _animator.SetTrigger(AnimRefarences.Blast);

            AbilityOn = true;
            AbilityOnInvoke();
            
            if (_bodyAngler && _arm) {
                _bodyAngler.RotatePart(_arm, Mathf.Atan2(_direction.y, _direction.x)*Mathf.Rad2Deg + 90);
            }
        }
        else {
            AbilityOn = false;
        }
    }
}
