using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/*
 * The basic AI for character enemies.
 * This enemy tires to walk to the player and punch it.
 */
public class AIBase : MonoBehaviour, IManager
{
    [SerializeField] protected SoundManager soundManager;
    [SerializeField] protected Transform frontCheck;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected CharacterController2D controller;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Transform punch;
    [SerializeField] protected AttackManagerBase attackManager;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask playerInvisibleLayer;
    [SerializeField] protected float detectRadius;
    [SerializeField] protected float attackDetectRadius;
    [SerializeField] protected float maintainDistance;
    [SerializeField] protected float maintainErrorRange;
    [SerializeField] protected float minMoveToDistance;
    [SerializeField] protected Ability ability;
    [SerializeField] protected int xpWorth;
    [SerializeField] protected CharacterStats characterStats;
    [SerializeField] protected bool startWalkingDirectionRight;
    [SerializeField] protected float timeToForgetPlayer;


    protected readonly Random _random = new Random();

    protected int _walkDirectionMult = 1;
    protected float _horizontalSpeed;
    protected float _nextCanAttack = 0;
    protected bool _disabled = false;
    protected bool canFlip = true;
    protected bool _shouldJump = false;
    protected bool _falling = false;
    private Coroutine _flipRoutine;
    protected float _attackSpeed;
    protected float _walkingSpeed;


    protected bool _moveDisabled = false;
    protected bool _actionDisabled = false;
    
    protected float _width;
    protected float _hight;
    
    
    protected float forgetPlayerTime;
    protected Collider2D playerCollider;
    private Collider2D[] PlayerColliderNoneAlloc = new Collider2D[1];

    
    /*
     * Start is called at the beginning of the scene.
     * Set base stats from the character stats.
     * Set starting facing direction.
     * Set animator as enemy.
     **/
    private void Start() {
        SetAttackSpeed(characterStats.GetCharacterStats().AttackSpeed);
        _walkingSpeed = characterStats.GetCharacterStats().MoveSpeed * (float)(_random.NextDouble()*0.4+0.8);
        if (!startWalkingDirectionRight) {
            _walkDirectionMult = -1;
            controller.Flip();
        }
        animator.SetBool(AnimRefarences.IsEnemy, true);
        
        _width = Math.Abs(transform.position.x - frontCheck.position.x)*2;
        _hight = Math.Abs(transform.position.y - groundCheck.position.y)*2;
        
        
        AdditionalStart();
    }

    // On base do nothing. Inheriting AIs can use this to set up stuff on start.
    protected virtual void AdditionalStart() {
        return;
    }

    // Get the xp worth from defeating this enemy.
    public int GetXp() {
        return xpWorth;
    }
    
    public void DisableManager() {
        _disabled = true;
        _moveDisabled = true;
        _actionDisabled = true;
    }

    public void DisableActions() {
        _actionDisabled = true;
    }

    public void DisableMove() {
        animator.SetFloat(AnimRefarences.Speed, 0f);
        _moveDisabled = true;
    }

    public void DisableFlip() {
        controller.canTurn = false;
    }

    public void EnableManager() {
        _disabled = false;
        _moveDisabled = false;
        _actionDisabled = false;
        controller.canTurn = true;
    }

    public void EnableActions() {
        EnableManager();
    }

    public void EnableMove() {
        EnableManager();
    }

    public void AbilityAnimationTrigger() {
        ability.AnimationTrigger();
    }

    // Set the ability of this AI. (Default AI has no ability)
    public void SetAbility(Ability newPower) {
        ability = newPower;
    }
    
    public void SetAttackManager(AttackManagerBase newAttack) {
        attackManager = newAttack;
    }

    public void SetRunSpeed(float speed) {
        _walkingSpeed = speed;
    }

    public float GetRunSpeed() {
        return _walkingSpeed;
    }

    public void SetAttackSpeed(float newAttackSpeed) {
        _attackSpeed = newAttackSpeed;
        animator.SetFloat(AnimRefarences.AttackSpeed, newAttackSpeed);
        attackManager.SetAttackSpeed(newAttackSpeed);
    }
    
    public float GetAttackSpeed() {
        return _attackSpeed;
    }

    public void Fly(bool shouldFly) {
        controller.MakeFlying(shouldFly);
    }

    public bool IsFlying() {
        return controller.IsFlying();
    }

    /*
     * The logic loop of this and iterating AI:
     *     - If the AI is disabled, it does nothing.
     *     - Look for the player, and save it's collider.
     *     - Forget the player collider if enough time has passed.
     *     - If player collider exists run PlayerDetectedLogic.
     *     - Otherwise, just move normally.
     */
    private void Update() {
        if (_disabled) {
            return;
        }
        
        _horizontalSpeed = 0f;

        // DetectPlayer sets the playerCollider if found.
        DetectPlayer();
        
        if (Time.time > forgetPlayerTime && playerCollider) {
            playerCollider = null;
        }
        
        if (playerCollider) {
            PlayerDetectedLogic();
        }
        else{
            MoveSelf();
        }

        AdditionalUpdateLogic();

        animator.SetFloat(AnimRefarences.Speed,Math.Abs(_horizontalSpeed));
    }

    protected virtual void AdditionalUpdateLogic() {
        return;
    }

    /*
     * Basic logic in case the player's collider is known:
     *     - If player is in attack range, attack it with random chance
     *     - Otherwise, move to the player.
     */
    protected virtual void PlayerDetectedLogic() {
        if (IsPlayerInAttackRange()) {
            if(_random.NextDouble() < 0.5){
                DoAttack();
            }
        }
        else {
            MaintainDistanceFromPlayer();
        }
    }

    // Use the power, if can
    protected virtual void UseAbility(Vector3 direction) {
        if(_actionDisabled || characterStats.IsDead() || !ability || !ability.CanUseAbilityAgain()) return;
        
        ability.UseAbilityStart(direction);
    }

    protected virtual bool CanUseAbility() {
        return (ability && ability.CanUseAbilityAgain() && !_actionDisabled);
    }

    /* Find the player using an overlap circle.
     * If this is a new discovery play detect player sound.
     * Also, set the timer to forget the player.
     */
    protected void DetectPlayer() {
        bool wasKnown = playerCollider;
        
        // PlayerColliderNoneAlloc[0] = null;
        
        var playerColliderTemp = Physics2D.OverlapCircle(frontCheck.position, detectRadius, playerLayer);
        
        if (playerColliderTemp) {
            playerCollider = playerColliderTemp;
            forgetPlayerTime = Time.time + timeToForgetPlayer;
            if (!wasKnown) {
                soundManager.PlayAudio(SoundManager.SoundClips.EnemyDetected);
            }
        }
        else if (wasKnown) {
            // target lost
            if (Utils.IsObjectInLayerMask(playerCollider.gameObject, playerInvisibleLayer)) {
                forgetPlayerTime -= 5f*Time.deltaTime;
            }
        }
    }

    // Check if player is in attack range but not too close
    protected bool IsPlayerInAttackRange() {
        Vector2 checkPosition = transform.position;
        Vector2 closestPointToPlayer = playerCollider.bounds.ClosestPoint(checkPosition);
        float distanceFromPlayer = Vector2.Distance(closestPointToPlayer, checkPosition);
        if (distanceFromPlayer <= attackDetectRadius) {
            FaceTarget();
            return true;
        }

        return false;
    }

    protected virtual void MaintainDistanceFromPlayer(float distance) {
        if(_moveDisabled || playerCollider==null) return;
        bool stopWalking = false;

        var position = transform.position;
        var playerPosition = playerCollider.transform.position;
        if (position.x > playerPosition.x + distance + maintainErrorRange ||
            (position.x < playerPosition.x && position.x > playerPosition.x - distance + maintainErrorRange)) {
            _walkDirectionMult = -1;
        }
        else if (position.x < playerPosition.x - distance - maintainErrorRange ||
            (position.x > playerPosition.x && position.x < playerPosition.x + distance - maintainErrorRange)) {
            _walkDirectionMult = 1;
        }
        else {
            FaceTarget();
            stopWalking = true;
        }

        _horizontalSpeed = _walkingSpeed * _walkDirectionMult;
        // at this point, horizontal speed is 0 (see beginning of update)
        if (AtEdge() || AtWall() || stopWalking) {
            _horizontalSpeed = 0;
        }
    }

    protected virtual void MaintainDistanceFromPlayer() {
        MaintainDistanceFromPlayer(maintainDistance);
    }

    // Move to the player
    protected virtual void MoveToPlayer() {
        
        Vector3 checkPosition = transform.position;
        Vector3 playerPosition = playerCollider.transform.position;
        
        // if not facing the player, and not too close to the player, and enough time has passed since last flip: Flip
        if ((transform.position.x - playerPosition.x) * controller.GetFacingMult() >= 0 && 
            Vector2.Distance(playerPosition, checkPosition) >= minMoveToDistance && canFlip) {
            
            _walkDirectionMult *= -1;
            controller.Flip();
            canFlip = false;
            StartCoroutine(FlipCooldown());
        }

        // at this point, horizontal speed is 0 (see beginning of update)
        _horizontalSpeed = _walkingSpeed * _walkDirectionMult;
        if (AtEdge() || AtWall()) {
            _horizontalSpeed = 0;
        }
    }

    // call attack managers attack function.
    protected void DoAttack() {
        if(_actionDisabled) return;
        attackManager.Attack();
    }

    public void AttackAnimationTrigger() {
        attackManager.AttackTrigger();
    }

    // Move around, flip at walls and edges 
    protected virtual void MoveSelf() {
        if(_moveDisabled) return;

        if(controller.IsGrounded()) {
            _horizontalSpeed = _walkingSpeed * _walkDirectionMult;
            if (AtEdge() || AtWall()) {
                    _walkDirectionMult *= -1;
                    _horizontalSpeed *= -1;
            }
        }
        
        // dont move in air, just decided it worked nicer
        else {
            _horizontalSpeed = 0;
        }

        // update the animator about the new move speed
        animator.SetFloat(AnimRefarences.Speed, Math.Abs(_horizontalSpeed));
    }

    // Check if the character is at a wall
    protected bool AtWall() {
        
        Collider2D rayCastForward = Physics2D.OverlapCircle(
            (Vector2)transform.position + Vector2.right*(_walkDirectionMult*(_width/2)), 0.2f, whatIsGround);
        
        return rayCastForward;
    }

    // Check is the character is at an edge of the ground
    protected bool AtEdge() {
        // Front check is a bit in front of the collider
        var frontCheckPosition = transform.position + Vector3.right * (_walkDirectionMult*(_width/2));
        // ground check is at the bottom edge of the collider
        var groundCheckPosition = groundCheck.position;
        
        // float height = frontCheckPosition.y - groundCheckPosition.y;
        
        var rayCastFrontDown = Physics2D.Raycast(frontCheckPosition, 
            Vector2.down, _hight/2 + 0.5f, whatIsGround);


        //Check if the ground is by the front of your feet and you are just on a slope, if not you may be on an edge.
        // Using this so that slopes will not count as an edge but steps will.
        if (!rayCastFrontDown.collider) {
            float forwardDistance = (frontCheckPosition.x - groundCheckPosition.x);
            // look down closer to the collider, slopes will see ground there but not more forwards
            var rayCastFootDown = Physics2D.Raycast(groundCheckPosition + Vector3.right*forwardDistance/3,
                                                                Vector2.down, 0.5f, whatIsGround);

            if (rayCastFootDown.collider == null) {
                // you are at edge
                return true;
            }
        }

        return false;
    }

    // On fixed update, send the current moving commands to the character controller. Also play moving audio
    private void FixedUpdate() {
        if(_disabled){
            soundManager.StopAudio(SoundManager.SoundClips.Walk);
            return;
        }
        
        controller.Move(_horizontalSpeed * Time.fixedDeltaTime, false, _shouldJump);
        
        if ((_horizontalSpeed > .01 || _horizontalSpeed < -.01) && controller.IsGrounded()) {
            soundManager.PlayAudio(SoundManager.SoundClips.Walk);
        }
        else {
            soundManager.StopAudio(SoundManager.SoundClips.Walk);
        }
    }
    
    // Draw the varius distance raduis gizmos
    void OnDrawGizmosSelected() {
        if (punch == null) return;

        var position = transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, attackDetectRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, minMoveToDistance);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(position, detectRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(position, maintainDistance + maintainErrorRange);
        Gizmos.DrawWireSphere(position, maintainDistance - maintainErrorRange);
        
        Gizmos.color = Color.yellow;
        var frontCheckPosition = transform.position + Vector3.right * (_walkDirectionMult*(_width/2));
        var groundCheckPosition = groundCheck.position;
        float hight = frontCheckPosition.y - groundCheckPosition.y;
        Gizmos.DrawLine(frontCheckPosition, frontCheckPosition + Vector3.down*(hight + 0.5f));


        Gizmos.color = Color.green;
        var p = (Vector2) transform.position + Vector2.right * (_walkDirectionMult * (_width / 2));
        Gizmos.DrawWireSphere(p, 0.2f);
    }

    // In order to stop the enemy from going crazy when the player is right on top of it, set a cooldown for flipping
    protected IEnumerator FlipCooldown() {
        canFlip = false;
        yield return new WaitForSeconds(0.5f);
        canFlip = true;
    }

    public void FaceTarget() {
        if(!playerCollider) return;

        if (controller.GetFacingMult() > 0) {
            if (playerCollider.transform.position.x < transform.position.x) {
                _walkDirectionMult *= -1;
                controller.Flip();
            }
        }
        else {
            if (playerCollider.transform.position.x > transform.position.x) {
                _walkDirectionMult *= -1;
                controller.Flip();
            }
        }
    }

    public Vector3 GetDirectionToTarget() {
        if (playerCollider) {
            return playerCollider.transform.position - transform.position;
        }
        return new Vector3(controller.GetFacingMult(), 0, 0);
    }
}
