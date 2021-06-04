using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * DO NOT USE, OLD FUNCTION
 */
public class EnemyAttack : AttackManagerBase
{

    protected override void AttackStart() {

        _animator.SetTrigger(AnimRefarences.Punch01);

        _controller.StopHorizontal();
    }

    public override void AttackTrigger() {
        _controller.StopHorizontal();
        Collider2D[] hits = Physics2D.OverlapCircleAll(punch.position, hitRadius, enemyLayer);
        foreach (var hit in hits) {
            HitManager.GetTakeDamage(hit.gameObject)?.Damage(characterStats.GetCharacterStats().PunchDamage * damageMult,
                new Vector2(_controller.GetFacingMult()*pushForce.x, pushForce.y));
        }

        if (hits.Length > 0) {
            _controller.StopHorizontal();
        }
    }
}