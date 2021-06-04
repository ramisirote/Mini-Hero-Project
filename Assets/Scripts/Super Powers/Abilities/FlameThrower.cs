using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A flame thrower ability. Keep holding the power to keep it on.
 * Part of the Fire Powers class
 * When an enemy walks into the flame, puts a fire object on them dealing damage over time.
 * The longer you burn an enemy, the more damage it'll do.
 */
public class FlameThrower : Ability
{

    private ParticleSystem _particleSystem;
    private Collider2D _collider2D;
    [SerializeField] private float energyRequierdPerSecond;
    [SerializeField] private float damageAmountOverTime;
    [SerializeField] private float extraDamagePerSecond;
    [SerializeField] private int dotTicks;
    [SerializeField] private float minOnTime;
    [SerializeField] private Material material;
    [SerializeField] private ParticleSystemRenderer particleSystemRenderer;
    [SerializeField] private string[] hitTags;
    [SerializeField] private GameObject onFirePartical;

    private bool _canTurnOff = true;
    private bool wantsToTurnOff;
    private Transform effectPointTransform;
    private Animator _animator;
    private ParticleSystem.MainModule _particleSystemMain;
    private CharacterController2D _controller2D;
    
    private Transform _arm;
    private float _armDefaultRotationAngle;
    private BodyAngler _bodyAngler;
    
    protected override void AdditionalInit() {
        _collider2D = GetComponent<PolygonCollider2D>();
        _collider2D.enabled = false;
        SetUpParticleSystem();
        SetUpArmAngler();
        _animator = parentCharacter.GetComponent<Animator>();
        _controller2D = parentCharacter.GetComponent<CharacterController2D>();
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
        
        
        transform.localRotation = Quaternion.Euler(0,0,67.5f);
    }


    private void SetUpParticleSystem() {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystemMain = _particleSystem.main;

        particleSystemRenderer.material = material;
        
        var m = particleSystemRenderer.material;
        m.SetColor("_Color1", _color1);
        m.SetColor("_Color2", _color2);
        m.SetColor("_Color3", _color3);
        
        _particleSystem.Stop();
    }

    private void Update() {
        // power on, consume energy, if none left: stop power
        if (AbilityOn && _collider2D.enabled 
                    && !CharacterStats.UseEnergy(Time.deltaTime*
                                         (energyRequierdPerSecond+ CharacterStats.GetCharacterStats().EnergyRegen))) {
            SetAbilityOff();
        }
        // if all off conditions are met, turn off
        if (wantsToTurnOff && _canTurnOff && AbilityOn) {
            SetAbilityOff();
        }

        // move to arm position
        if (effectPointTransform) {
            transform.position = effectPointTransform.position;
        }
    }


    private IEnumerator DelayOn(Vector3 direction) {
        AbilityOn = true;
        AbilityOnInvoke();
        _particleSystem.Play();
        if(audioSource) audioSource.Play();
        
        Manager.DisableFlip();

        if (_bodyAngler && _arm) {
            _bodyAngler.RotatePart(_arm, Mathf.Atan2(direction.y, direction.x)*Mathf.Rad2Deg + 90f);
        }
        Manager.FaceTarget();
        
        _animator.SetBool(AnimRefarences.IsFireingContinues, true);
        Manager.DisableActions();
        _canTurnOff = false;
        
        yield return new WaitForSeconds(minOnTime);
        
        if(AbilityOn) _collider2D.enabled = true;
        _canTurnOff = true;
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn) {
            if (CharacterStats.UseEnergy(energyRequired)) {
                StartCoroutine(DelayOn(direction));
            }
        }
        else {
            wantsToTurnOff = false;
            _particleSystem.Play();
        }
    }
    

    public override void SetAbilityOff() {
        if(!AbilityOn) return;
        
        AbilityOn = false;
        wantsToTurnOff = false;
        _canTurnOff = false;
        if (_particleSystem) {
            _particleSystem.Stop();
            _particleSystem.Clear();
        }
        
        if(audioSource) audioSource.Stop();

        if (_bodyAngler && _arm) {
            _bodyAngler.RotatePart(_arm, _armDefaultRotationAngle);
        }
        
        _collider2D.enabled = false;
        Manager.EnableManager();
        _animator.SetBool(AnimRefarences.IsFireingContinues, false);
        AbilityOffInvoke();
    }
    
    public override void UseAbilityRelease(Vector3 direction) {
        if(AbilityOn) wantsToTurnOff = true;
    }


    private void OnTriggerEnter2D(Collider2D other) {
        
        GameObject otherGameObject = other.gameObject;
        bool hasHitTag = false;
        foreach (var tags in hitTags) {
            if (otherGameObject.CompareTag(tags)) {
                hasHitTag = true;
                break;
            }
        }
        if(!hasHitTag) return;
        
        // var damager = otherGameObject.GetComponent<TakeDamage>();
        // if (damager) {
        //     // damager.Damage(damageAmount, _controller2D.GetFacingMult());
        //     damager.DamageOverTime(damageAmountOverTime, dotTicks);
        // }

        OnFire.MakeOnFire(otherGameObject, dotTicks, onFirePartical, extraDamagePerSecond, 
            damageAmountOverTime, particleSystemRenderer.material);
    }

    // private void MakeOnFire(GameObject otherGameObject) {
    //     var onFire = otherGameObject.GetComponentInChildren<OnFire>();
    //     if (onFire) {
    //         if (onFire.IsTicking()) {
    //             onFire.ExtendFireDuration(Time.fixedDeltaTime*2);
    //             onFire.AddDamageAmount(extraDamagePerSecond*Time.fixedDeltaTime*2);
    //         }
    //         else {
    //             onFire.ExtendFireDuration(dotTicks);
    //             onFire.SetDamageAmount(damageAmountOverTime);
    //         }
    //     }
    //     else if(onFirePartical){
    //         var onFireParticalInst = Instantiate(onFirePartical, otherGameObject.transform);
    //         onFire = onFireParticalInst.GetComponent<OnFire>();
    //         onFire.SetDamageAmount(damageAmountOverTime);
    //         onFire.ExtendFireDuration(dotTicks);
    //         onFire.SetFireMaterial(particleSystemRenderer.material);
    //     }
    // }

    private void OnTriggerStay2D(Collider2D other) {
        GameObject otherGameObject = other.gameObject;
        bool hasHitTag = false;
        foreach (var tags in hitTags) {
            if (otherGameObject.CompareTag(tags)) {
                hasHitTag = true;
                break;
            }
        }
        if(!hasHitTag) return;
        
        OnFire.MakeOnFire(otherGameObject, dotTicks, onFirePartical, extraDamagePerSecond, 
            damageAmountOverTime, particleSystemRenderer.material);
    }

    protected override void OnDamageTaken(object o, float damageAmount) {
        if (_particleSystem) {
            SetAbilityOff();
        }
    }

    public override void UpdateDirection(Vector3 direction) {
        float angle;
        Manager.FaceTarget();
        angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90 )* -Controller.GetFacingMult();
        
        
        _particleSystemMain.startRotation = (_arm.transform.localRotation.eulerAngles.z + 90)* Mathf.Deg2Rad;
        
        if (_bodyAngler && _arm) {
            _bodyAngler.RotatePart(_arm, angle);
        }
    }
}
