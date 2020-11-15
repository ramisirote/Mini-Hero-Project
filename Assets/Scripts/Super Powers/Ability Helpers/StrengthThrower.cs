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

    private GameObject parent;
    private IManager parentManager;
    private float doneTime;
    private Rigidbody2D rb;
    private Vector3 _velocity = Vector3.zero;
    private CharacterStats _stats;

    private bool pushed;

    private Vector2 _direction;

    
    public void Init(LayerMask oEnemyLayer, float oDamage, float oFlytime, float oThrowForce, Vector2 oDirection) {
        enemyLayer = oEnemyLayer;
        damage = oDamage;
        flyTime = oFlytime;
        throwForce = oThrowForce;
        _direction = oDirection;
        
        parent = transform.parent.gameObject;
        parentManager = parent.GetComponent<IManager>();
        parentManager.DisableManager();
        doneTime = Time.time + flyTime;
        pushed = false;
        rb = parent.GetComponent<Rigidbody2D>();
        _stats = parent.GetComponent<CharacterStats>();
    }

    private void Update() {
        if (Time.time > doneTime) {
            if (_stats.IsDead()) {
                rb.velocity = Vector2.zero;
            }
            else {
                parentManager.EnableManager();
            }
            Destroy(gameObject);
        }
        else {
            parentManager.DisableManager();
        }
    }

    private void FixedUpdate() {
        if (!pushed) {
            pushed = true;
            rb.AddForce(throwForce*_direction/_direction.magnitude);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        var g = other.gameObject;
        int facing;
        facing = _direction.x > 0 ? 1 : 0;
        
        // The tag should be changed in the inspector so that in can work on players too.
        // Or not. Could be interesting if enemies could friendly fire with this?
        if (g != parent && g.CompareTag(parent.tag)) {
            g.GetComponent<TakeDamage>().Damage(damage, facing);
        }
    }
}
