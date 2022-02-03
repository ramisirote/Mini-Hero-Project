using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script goes on the game object that is attached to a character hit by the super strength basic attack.
 * When attacked to a character, moves them in the direction. When collides with a enemy, deals damage to them.
 */
public class StrengthThrower : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float flyTime;
    [SerializeField] private float throwForce;
    [SerializeField] private float damage;
    [SerializeField] private bool hitMultiple;

    private GameObject parent;
    private IManager parentManager;
    private float doneTime;
    private Rigidbody2D rb;
    private Vector3 _velocity = Vector3.zero;
    private CharacterStats _stats;
    private CharacterController2D _controller;

    private bool pushed;

    private Vector2 _direction;

    
    public void Init(LayerMask oEnemyLayer, float oDamage, float oFlytime, float oThrowForce, Vector2 oDirection,
        bool oHitMultiple = false) {
        enemyLayer = oEnemyLayer;
        damage = oDamage;
        flyTime = oFlytime;
        throwForce = oThrowForce;
        _direction = oDirection;
        
        parent = transform.parent.gameObject;
        parentManager = parent.GetComponent<IManager>();
        _controller = parent.GetComponent<CharacterController2D>();
        parentManager.Stunned();
        doneTime = Time.time + flyTime;
        if (_controller.IsFlying()) doneTime += flyTime * 0.3f;
        pushed = false;
        rb = parent.GetComponent<Rigidbody2D>();
        _stats = parent.GetComponent<CharacterStats>();
        hitMultiple = oHitMultiple;
    }

    private void Update() {
        if (Time.time < doneTime || (!_controller.IsGrounded() && !_controller.IsFlying())) {
            parentManager.Stunned();
        }
        else {
            EndThrow();
        }
    }

    private void EndThrow() {
        if (_stats.IsDead()) {
            rb.velocity = Vector2.zero;
        }
        else {
            parentManager.Stunned(false);
        }
        Destroy(gameObject);
    }

    private void FixedUpdate() {
        if (!pushed) {
            pushed = true;
            if (_direction.magnitude > 0) {
                rb.AddForce(throwForce*_direction/_direction.magnitude);
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        var g = other.gameObject;
        int facing;
        facing = _direction.x > 0 ? 1 : 0;
        
        // The tag should be changed in the inspector so that in can work on players too.
        // Or not. Could be interesting if enemies could friendly fire with this?
        if (g != parent && Utils.IsObjectInLayerMask(g, enemyLayer)) {
            HitManager.GetTakeDamage(g)?.Damage(damage, facing);
            HitManager.GetTakeDamage(parent)?.Damage(damage*0.3f, -facing);
            if (!hitMultiple) {
                EndThrow();
            }
        }
        else if(g.CompareTag("Ground")) {
            HitManager.GetTakeDamage(parent)?.Damage(damage, -facing);
            EndThrow();
        }
    }
}
