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
    private float _direction;
    private IManager _manager;
    private GameObject _thrower;
    private float _cameraShake;
    private bool _isPlayer;
    
    private float _throwerDamage;
    private float _flyTime;
    private float _flyForce;
    private LayerMask _enemyLayer;

    private float _actualDamage;
    
    /*
     * This Attack Manager is set up by the super strength ability, not from the inspector
     */
    public void Init(Animator newAnimator, CharacterController2D newController, CharacterStats newCharacterStats,
                        IManager manager,float newCooldown, GameObject newThrower, float newDamageMult,
                        float cameraShake,bool isPlayer) {
        _animator = newAnimator;
        _controller = newController;
        characterStats = newCharacterStats;
        _manager = manager;
        cooldown = newCooldown;
        _thrower = newThrower;
        damageMult = newDamageMult;
        _isPlayer = isPlayer;
        _cameraShake = cameraShake;

        _actualDamage = characterStats.GetCharacterStats().PunchDamage * damageMult;
    }

    public void InitThrower(LayerMask oEnemyLayer, float oDamage, float oFlytime, float oThrowForce) {
        _throwerDamage = oDamage;
        _flyForce = oThrowForce;
        _enemyLayer = oEnemyLayer;
        _flyTime = oFlytime;
    }

    /*
     * Runs the decided animation and stops the character.
     */
    protected override void AttackStart() {
        _animator.SetTrigger(AnimRefarences.Punch03);
        _controller.StopHorizontal();
        // _manager.FaceTarget();
    }
    
    /*
     * Attack Trigger is triggered by the animation.
     * In This case the hit also creates a thrower object on the 
     */
    public override void AttackTrigger() {
        _controller.StopHorizontal();
        var hit = Physics2D.OverlapCircle(punch.position, hitRadius, enemyLayer);
        if (hit) {
            if (hit.GetComponent<CharacterStats>().IsDead()) return;
            
            HitManager.GetTakeDamage(hit.gameObject)?.Damage(_actualDamage, Vector2.zero);
            if(_isPlayer) CinemachineShake.Instance.ShakeCamera(_cameraShake);
            
            // The thrower handles the push and damage parts of the super strength hit, as well as
            // the collision with other enemies
            hit.GetComponent<IManager>().DisableManager();
            var throwerInstance = Instantiate(_thrower, hit.gameObject.transform);
            throwerInstance.GetComponent<StrengthThrower>().Init(_enemyLayer, _actualDamage, _flyTime, 
                _flyForce, _manager.GetDirectionToTarget());
        }
    }

}
