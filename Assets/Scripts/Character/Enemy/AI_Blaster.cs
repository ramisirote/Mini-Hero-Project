using UnityEngine;

/*
 * This enemy tries to hit the player with a simple ray-cast blast attack.
 */
public class AI_Blaster : AIBase
{

    protected override void PlayerDetectedLogic() {
        if (IsPlayerInAttackRange()) {
            if(Time.time >= _nextCanAttack && _random.NextDouble() < 0.2){
                _nextCanAttack = Time.time + 1 / characterStats.GetCharacterStats().AttackRate;
                DoAttack();
            }
        }
        else if(PlayerInBlastRang() && CanUseAbility()) {
            var direction = _player.transform.position - transform.position;
            UseAbility(direction);
        }
        else {
            MaintainDistanceFromPlayer();
        }
    }

    private bool PlayerInBlastRang() {
        var position = transform.position;
        var direction = _player.transform.position - position;
        var ray = Physics2D.Raycast(position, direction, playerLayer);

        return ray.collider;
    }

    protected override void UseAbility(Vector3 direction) {
        if (CanUseAbility()) {
            ability.UseAbility(direction);
        }
    }
}
