
using System.Collections;
using UnityEngine;

/*
 * A simple ray-cast line attack that hit enemies at a distance.
 * Part of the Energy Power class.
 * Strengths: Range, low cooldown, easy to use and aim, fast.
 * Weaknesses: Low damage, high energy cost per-damage, short disable.
 *
 * This ability uses the animation trigger to tell when to do the blast, and then again when to turn off the blast.
 */
public class EnergyBlast : Ability
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
                    upperArmRotationDefault = _arm.localRotation.eulerAngles;
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
        Manager.FaceTarget();
        if (!_bodyAngler && _arm) {
            _arm.eulerAngles = new Vector3(0,0,45*Controller.GetFacingMult());
        }
        
        energyLine.enabled = true;
        energyLine.SetPosition(0, transform.position);
        
        if(audioSource) audioSource.Play();
        if(parentCharacter.CompareTag("Player")) CinemachineShake.Instance.ShakeCamera();

        BlastHit();
        
        energyLine.enabled = true;
        StartCoroutine(MakeLineInvisibleFor(lineVisibleFor));
        NextCanUse = Time.time + abilityCooldown;
        
        _startPowEffect.Play();
    }

    private void BlastHit() {
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, _direction, 100, enemyLayer);
        if (rayHit.collider) {
            energyLine.SetPosition(1, rayHit.point);
            var enemyHit = HitManager.GetTakeDamage(rayHit.collider.gameObject);
            if (enemyHit != null) {
                enemyHit.Damage(damage, Controller.GetFacingMult());
            }

            _endPowEffect.transform.position = rayHit.point;
            _endPowEffect.Play();
        }
        else {
            Vector2 endTarget = transform.position + parentCharacter.transform.right * 100f;
            energyLine.SetPosition(1, endTarget);
        }
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
            Manager.FaceTarget();
            
            Controller.StopHorizontal();
            _direction = vectorToTarget;
            
            _animator.SetTrigger(AnimRefarences.Blast);

            AbilityOn = true;
            AbilityOnInvoke();
            
            if (_bodyAngler && _arm) {
                // if (Controller.GetFacingMult() == 1) {
                //     _bodyAngler.RotatePart(_arm, Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg + 180);
                // }
                // else {
                //     _bodyAngler.RotatePart(_arm, Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg);
                // }
                _bodyAngler.RotatePart(_arm, -Controller.GetFacingMult()*
                                                    Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg
                                                    + 90 + 90*Controller.GetFacingMult());
            }
        }
        else {
            AbilityOn = false;
        }
    }
}
