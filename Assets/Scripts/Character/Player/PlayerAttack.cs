using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The player attack. It has two attack animations that cycle if triggered within a window.
 * Other then that, it's a basic attack.
 */
public class PlayerAttack : AttackManagerBase
{
    [SerializeField] private float resetStateTimeLength;
    [SerializeField] private float secondaryCooldown;
    [SerializeField] private float cameraShake;
    private float _animationState = 0;

    private float resetStateTime;
    
    protected override void AttackStart() {
        if (Time.time > resetStateTime) {
            _animationState = 0;
        }

        switch (_animationState) {
            case 0:
                _animator.SetTrigger(AnimRefarences.Punch01); break;
            case 1:
                _animator.SetTrigger(AnimRefarences.Punch02);
                timeCanNextAttack = Time.time + secondaryCooldown/_attackSpeed;
                break;
        }

        _animationState = (_animationState + 1) % 2;
        resetStateTime = Time.time + resetStateTimeLength;
        _controller.StopHorizontal();
    }

    public override void AttackTrigger() {
        _controller.StopHorizontal();
        Collider2D[] hits = Physics2D.OverlapCircleAll(punch.position, hitRadius, enemyLayer);
        foreach (var hit in hits) {
            hit.GetComponent<TakeDamage>().Damage(characterStats.GetCharacterStats().PunchDamage*damageMult, 
                                             _controller.GetFacingMult()*Vector2.right*pushForce);
        }
        if (hits.Length > 0) {
            _controller.StopHorizontal();
            if(gameObject.CompareTag("Player")) CinemachineShake.Instance.ShakeCamera();
        }
        
        resetStateTime = Time.time + resetStateTimeLength;
    }
}
