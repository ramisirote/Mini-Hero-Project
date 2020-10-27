using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AI_Blaster : AIBase
{
    
    protected override void PlayerDetectedLogic() {
        if (IsPlayerInAttackRange()) {
            if(Time.time >= _nextCanAttack && _random.NextDouble() < 0.2){
                _nextCanAttack = Time.time + Time.deltaTime / characterStats.GetCharacterStats().AttackRate;
                DoAttack();
            }
        }
        else if(PlayerInBlastRang()) {
            var direction = playerCollider.transform.position - transform.position;
            UseAbility(direction);
        }
        else {
            MoveToPlayer();
        }
    }

    private bool PlayerInBlastRang() {
        var position = transform.position;
        var direction = playerCollider.transform.position - position;
        var ray = Physics2D.Raycast(position, direction, playerLayer);

        return ray.collider;
    }

    protected override void UseAbility(Vector3 direction) {
        if (ability && ability.CanUseAbilityAgain() && !_actionDisabled) {
            ability.UseAbility(direction);
        }
    }
}
