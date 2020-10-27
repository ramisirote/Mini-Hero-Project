using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * Managers the basic attack of the character. Its an abstract class that handles the basics.
 */
[System.Serializable]
public abstract class AttackManagerBase: MonoBehaviour
{
    [SerializeField] protected float damageMult;
    [SerializeField] protected Transform punch;
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] protected float hitRadius;
    [SerializeField] protected Vector2 pushForce;
    [SerializeField] protected float cooldown;

    [SerializeField] protected Animator _animator;
    [FormerlySerializedAs("_stats")] [SerializeField] protected CharacterStats characterStats;
    [SerializeField] protected CharacterController2D _controller;

    protected float _attackSpeed;
    
    protected float timeCanNextAttack;

    // Set the the punch Transform.
    public void SetPunchTransform(Transform newPunch) {
        punch = newPunch;
    }
    
    // Set the attack speed
    public void SetAttackSpeed(float attackSpeed) {
        _attackSpeed = attackSpeed;
    }

    // Do the attack if attack cooldown is done. Resetting the timer.
    public virtual void Attack() {
        if (Time.time > timeCanNextAttack) {
            timeCanNextAttack = Time.time + cooldown;
            AttackStart();
        }
    }

    // return if the attack cooldown is done.
    public bool CanAttack() {
        return Time.time > timeCanNextAttack;
    }

    // Start the attack. Mostly plays the animation
    protected abstract void AttackStart();

    // Get the trigger that the damage part of the attack is triggered.
    public abstract void AttackTrigger();
    
    // Draw the radius of the attack.
    void OnDrawGizmosSelected() {
        if (punch == null) return;
        
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(punch.position, hitRadius);
    }
}
