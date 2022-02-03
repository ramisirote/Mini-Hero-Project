using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MartialArtAttaker : AttackManagerBase
{
    [SerializeField] private float resetStateTimeLength;
    [SerializeField] private float cameraShake;
    [SerializeField] private Vector2[] pushVectors = new Vector2[5];
    [SerializeField] private float[] cooldowns = new float[4];
    [SerializeField] private float jumpHit;
    private int _animationState = 0;

    private float resetStateTime;
    
    private float _direction;
    private GameObject _thrower;
    private bool _isPlayer;
    
    private LayerMask _enemyLayer;

    private float _actualDamage;
    
    private int _numberOfStates = 4;

    private void Start() {
        _actualDamage = characterStats.GetCharacterStats().PunchDamage * damageMult;
        _numberOfStates = cooldowns.Length;
    }

    public void Init(Animator newAnimator, CharacterController2D newController, CharacterStats newCharacterStats,
        float[] newCooldown, Vector2[] newPushVectors, float newDamageMult, float newCameraShake,float hitJump, bool isPlayer) {
        _animator = newAnimator;
        _controller = newController;
        characterStats = newCharacterStats;
        cooldowns = newCooldown;
        damageMult = newDamageMult;
        _isPlayer = isPlayer;
        cameraShake = newCameraShake;
        pushVectors = newPushVectors;
        jumpHit = hitJump;

        _actualDamage = characterStats.GetCharacterStats().PunchDamage * damageMult;

        _numberOfStates = cooldowns.Length;
    }
    
    protected override void AttackStart(string animation=null) {
        if (Time.time > resetStateTime) {
            _animationState = 0;
        }
        else {
            _animationState %= _numberOfStates;
        }

        if (animation is null){
            switch (_animationState) {
                case 0: _animator.SetTrigger(AnimRefarences.Punch01); break;
                case 1: _animator.SetTrigger(AnimRefarences.Punch02); break;
                case 2: _animator.SetTrigger(AnimRefarences.Punch05); break;
                case 3: _animator.SetTrigger(AnimRefarences.Punch04); break;
            }
        } else {
            _animator.SetTrigger(animation);
        }
        timeCanNextAttack = Time.time + cooldowns[_animationState]/_attackSpeed;
        
        _animationState++;
        resetStateTime = Time.time + resetStateTimeLength;
        if(_controller.IsGrounded()){
            _controller.StopHorizontal();
        }
    }

    public override void AttackTrigger() {
        if (_animationState == _numberOfStates) {
            _controller.Push(new Vector2(jumpHit*_controller.GetFacingMult(), jumpHit));
        }
        else {
            if(_controller.IsGrounded()){
                _controller.StopHorizontal();
            }
        }

        if (_animationState < 1) _animationState = 1;
        
        var hitPush = new Vector2(_controller.GetFacingMult() * pushVectors[_animationState - 1].x, pushVectors[_animationState - 1].y);
        if (_animationState < _numberOfStates && !_controller.IsGrounded()) {
            hitPush = new Vector2(_controller.GetFacingMult() * pushVectors[_numberOfStates].x, pushVectors[_numberOfStates].y);
        }

        StartCoroutine(HitWindow(hitPush));

        resetStateTime = Time.time + resetStateTimeLength;
    }


    IEnumerator HitWindow(Vector2 hitPush) {
        var windowDoneTime = Time.time + 0.05f;
        bool hitEffectsDone = false;
        
        while (Time.time < windowDoneTime) {
            Collider2D[] hits = Physics2D.OverlapCircleAll(punch.position, hitRadius, enemyLayer);
            foreach (var hit in hits) {
                HitManager.GetTakeDamage(hit.gameObject).Damage(_actualDamage, hitPush);
            }
            
            // only do this once
            if (hits.Length > 0 && !hitEffectsDone) {
                hitEffectsDone = true;
                _controller.StopHorizontal();
                if(gameObject.CompareTag("Player")) CinemachineShake.Instance.ShakeCamera(cameraShake);
            }
            yield return null;
        }
    }
}
