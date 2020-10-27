using UnityEngine;

/*
 * This enemy tries to hit the player with a simple ray-cast blast attack.
 */
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
