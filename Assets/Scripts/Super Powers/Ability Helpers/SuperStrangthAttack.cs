using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The basic attack active while the Super Strength ability is active.
 * When hit makes a (not visible) object on the character hit that
 * pushes them and deals damage to all enemies hit while pushed this way.
 */
public class SuperStrangthAttack : AttackManagerBase
{
    private float direction;
    private IManager manager;
    private GameObject thrower;
    
    /*
     * This Attack Manager is set up by the super strength ability, not from the inspector
     */
    public void Init(Animator newAnimator, CharacterController2D newController, CharacterStats newCharacterStats,
                        IManager manager,float newCooldown, GameObject newThrower, float newDamageMult) {
        _animator = newAnimator;
        _controller = newController;
        characterStats = newCharacterStats;
        this.manager = manager;
        cooldown = newCooldown;
        thrower = newThrower;
        damageMult = newDamageMult;
    }

    /*
     * Runs the decided animation and stops the character.
     */
    protected override void AttackStart() {
        _animator.SetTrigger(AnimRefarences.Punch03);
        _controller.StopHorizontal();
        manager.FaceTarget();
    }
    
    /*
     * Attack Trigger is triggered by the animation.
     * In This case the hit also creates a thrower object on the 
     */
    public override void AttackTrigger() {
        _controller.StopHorizontal();
        var hit = Physics2D.OverlapCircle(punch.position, hitRadius, enemyLayer);
        if (hit) {
            hit.GetComponent<TakeDamage>().Damage(characterStats.GetCharacterStats().PunchDamage * damageMult, Vector2.zero);
            
            if (!hit.GetComponent<CharacterStats>().IsDead()) { // is the hit target is dead, dont push it
                // The thrower handles the push and damage parts of the super strength hit, as well as
                // the collision with other enemies
                hit.GetComponent<IManager>().DisableManager();
                var throwerInstance = Instantiate(thrower, hit.gameObject.transform);
                throwerInstance.GetComponent<StrengthThrower>().direction = manager.GetDirectionToTarget();
            }
        }
    }

}
