using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * An AI that has the energy charge bolt ability.
 * This enemy has ranged capability.
 */
public class AI_ChargeBolt : AIBase
{

    [SerializeField] private float chargeTime;
    [SerializeField] private float timeBetweenCharges;

    private float _doneChargingTime = 0;

    private float nextCanUseAbility = 0;

    private TakeDamage damager;

    protected override void AdditionalStart() {
        damager = GetComponent<TakeDamage>();
        damager.OnDamage += OnDamage;
        
        ability.Init(gameObject);
    }

    private Coroutine _coroutine;

    private void OnDamage(object sender, float e) {
        if (ability.IsAbilityOn()) {
            nextCanUseAbility = Time.time + timeBetweenCharges;
            ability.UseAbilityRelease(GetDirectionToTarget());
        }
            
    }


    // The detect player logic
    protected override void PlayerDetectedLogic() {
        // If charging bolt, fire after charged enough. Keeps power pointing at target.
        if (ability.IsAbilityOn()) {
            // Check if the player is in a straight line with no ground in the way.
            RaycastHit2D playerHit = Physics2D.Raycast(transform.position, GetDirectionToTarget(), 100f,
                whatIsGround+playerLayer);
            Debug.Log(playerHit.collider.gameObject);
            if (Time.time > _doneChargingTime && playerHit.collider.gameObject.CompareTag("Player")) {
                ability.UseAbilityRelease(GetDirectionToTarget());
                nextCanUseAbility = Time.time + timeBetweenCharges;
                _doneChargingTime = Time.time + chargeTime;
            }
            else {
                ability.UpdateDirection(GetDirectionToTarget());
                FacePowerTarget();
            }
        }
        // If you can attack the player, just attack him, otherwise start charging the ability.
        else {
            if (IsPlayerInAttackRange() && Time.time >= _nextCanAttack && _random.NextDouble() < 0.5) {
                _nextCanAttack = Time.time + 1 / characterStats.GetCharacterStats().AttackRate;
                DoAttack();
            }
            else {
                if (Time.time > nextCanUseAbility) {
                    UseAbility(GetDirectionToTarget());
                }
                else {
                    MoveToPlayer();
                }
            }
        }

    }

    // Use the ability and update the next time you can use the ability.
    protected override void UseAbility(Vector3 direction) {
        if(_actionDisabled || characterStats.IsDead() ) return;
        
        ability.UseAbilityStart(direction);
        nextCanUseAbility = Time.time + timeBetweenCharges;
        _doneChargingTime = Time.time + chargeTime;
    }
    
}
