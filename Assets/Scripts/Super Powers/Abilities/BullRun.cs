using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullRun : Ability
{

    [SerializeField] private float minOnTime;
    [Range(0,2)][SerializeField] private float speed;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float lookForwardDistance;
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private float damage;
    [SerializeField] private Vector2 pushForce;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private ParticleSystem airCushin;
    
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private float canTurnOffTime;
    private bool _canTurnOff;
    private Collider2D _collider;
    private TakeDamage _takeDamage;
    protected override void AdditionalInit() {
        _animator = parentCharacter.GetComponent<Animator>();
        transform.parent = parentCharacter.transform;
        transform.position = parentCharacter.transform.position;
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
        _takeDamage = parentCharacter.GetComponent<TakeDamage>();
        trail.colorGradient = Utils.CreateGradient(new []{_color1, _color2, _color3}, new []{1f,1f,1f});
        trail.enabled = false;
        airCushin.Stop();
        var airCushinMain = airCushin.main;
        airCushinMain.startColor = new Color(_color1.r, _color1.g, _color1.b, 0.1f);
    }

    public override void UseAbility(Vector3 direction) {
        if (AbilityOn) return;
        AbilityOn = true;
        AbilityOnInvoke();
        Manager.DisableManager();
        canTurnOffTime = Time.time + minOnTime;
        _canTurnOff = false;
        _collider.enabled = true;
        
        _animator.SetBool(AnimRefarences.BullRun, true);
        _takeDamage.SetPlayAnimatoin(false);
        trail.Clear();
        trail.enabled = true;
        airCushin.Clear();
        airCushin.Play();
    }

    private void Update() {
        if (AbilityOn) {
            var hit = Physics2D.Raycast(transform.position, Vector2.right * Controller.GetFacingMult(), 
                lookForwardDistance, whatIsGround);
            if (hit.collider) {
                SetAbilityOff();
            }

            if (Time.time > canTurnOffTime && _canTurnOff) {
                SetAbilityOff();
            }
        }
    }

    private void FixedUpdate() {
        if (AbilityOn) {
            Controller.Move(Controller.GetFacingMult()*speed, false, false);
        }
    }

    public override void UseAbilityRelease(Vector3 direction) {
        if (AbilityOn) _canTurnOff = true;
    }

    public override void SetAbilityOff() {
        if (!AbilityOn) return;
        Manager.EnableManager();
        _animator.SetBool(AnimRefarences.BullRun, false);
        _collider.enabled = false;
        _canTurnOff = false;
        AbilityOn = false;
        _takeDamage.SetPlayAnimatoin(true);
        trail.enabled = false;
        airCushin.Stop();
        AbilityOffInvoke();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (Utils.IsObjectInLayerMask(other.gameObject, whatIsEnemy)) {
            other.gameObject.GetComponent<ITakeDamage>().Damage(damage, new Vector2(pushForce.x*Controller.GetFacingMult(), pushForce.y));
        }
    }
}
