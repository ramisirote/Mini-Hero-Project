using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedDash : Ability
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Collider2D hitCollider;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float dashForce;
    [SerializeField] private float damage;
    [SerializeField] private Vector2 pushForce;
    [SerializeField] private float dashTime;

    private Rigidbody2D _patentRigidBody;
    private Animator _animator;
    private float _defaultGravity = 1;
    private bool _shouldAddForce = false;
    private float _stopDashingTime;

    protected override void AdditionalInit() {
        _animator = parentCharacter.GetComponent<Animator>();
        _patentRigidBody = parentCharacter.GetComponent<Rigidbody2D>();
        _defaultGravity = _patentRigidBody.gravityScale;
        trailRenderer.colorGradient = Utils.CreateGradient(new []{_color1, _color2, _color3},new []{0f, 0.5f,1f});
        trailRenderer.enabled = false;
        hitCollider.enabled = false;
        transform.position = parentCharacter.transform.position;
    }

    public override void UseAbility(Vector3 direction) {
        if (AbilityOn) { return; }

        AbilityOn = true;
        Manager.DisableManager();
        Manager.DisableFlip();
        hitCollider.enabled = true;
        _shouldAddForce = true;
        _patentRigidBody.gravityScale = 0;
        trailRenderer.enabled = true;
        _stopDashingTime = Time.time + dashTime;
        
        _animator.SetBool(AnimRefarences.Dash, true);
        
        NextCanUse = Time.time + abilityCooldown;
        
        AbilityOnInvoke();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (Utils.IsObjectInLayerMask(other.gameObject, enemyLayer) && AbilityOn) {
            var damager = other.gameObject.GetComponent<ITakeDamage>();
            damager?.Damage(damage, pushForce*Controller.GetFacingMult());
        }
    }

    private void Update() {
        if (AbilityOn && Time.time > _stopDashingTime) {
            SetAbilityOff();
        }
    }

    private void FixedUpdate() {
        if (_shouldAddForce) {
            _patentRigidBody.velocity = Vector2.zero;
            _patentRigidBody.AddForce(new Vector2(dashForce*Controller.GetFacingMult(), -10));
            _shouldAddForce = false;
        }
    }

    public override void SetAbilityOff() {
        Manager.EnableManager();
        hitCollider.enabled = false;
        _patentRigidBody.gravityScale = _defaultGravity;
        _shouldAddForce = false;
        trailRenderer.Clear();
        trailRenderer.enabled = false;
        _stopDashingTime = Time.time;
        _animator.SetBool(AnimRefarences.Dash, false);
        AbilityOn = false;
        AbilityOffInvoke();
    }
}
