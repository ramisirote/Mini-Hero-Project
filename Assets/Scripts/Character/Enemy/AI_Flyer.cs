using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = System.Random;

/*
 * A flying enemy.
 * Patrols an area. (Left and right)
 * Moves to the player and tries to punch.
 */
public class AI_Flyer : AIBase
{
    [SerializeField] private float verticalMaintainDistance;
    [SerializeField] private float leftPatrolDistance;
    [SerializeField] private float rightPatrolDistance;
    
    private float _verticalSpeed = 0f;
    private float patrolBorderLeft;
    private float patrolBorderRight;
    private float _moveEdgeLeftGizmo;
    private float _moveEdgeRightGizmo;
    
    // Set controller and animation to flying.
    // Set up flying patrol border
    protected override void AdditionalStart() {
        animator.SetBool(AnimRefarences.Flying, true);
        controller.MakeFlying(true);

        var startPositionX = transform.position.x;
        patrolBorderLeft = startPositionX - leftPatrolDistance;
        patrolBorderRight = startPositionX + rightPatrolDistance;
    }
    
    protected override void MoveSelf() {
        
        if (AtWall() || AtEndOfPatrol()) {
            _walkDirectionMult *= -1;
        }
        
        _horizontalSpeed = _walkingSpeed * _walkDirectionMult;
        animator.SetFloat(AnimRefarences.Speed, Math.Abs(_horizontalSpeed));

        if (_verticalSpeed > 0.1f || _verticalSpeed < -0.1f) {
            _verticalSpeed = -_verticalSpeed/1.5f;
        }
        else {
            _verticalSpeed = 0f;
        }
    }
    
    protected override void MaintainDistanceFromPlayer() {
        if(_moveDisabled || playerCollider==null) return;
        bool stopHorizontal = false;

        // Maintain vertical distance
        var position = transform.position;
        var playerPosition = playerCollider.transform.position;
        if (position.y > playerPosition.y + verticalMaintainDistance + maintainErrorRange ||
            (position.y < playerPosition.y && position.y > playerPosition.y - verticalMaintainDistance + maintainErrorRange)) {
            _verticalSpeed = -1 * _walkingSpeed;
        }
        else if (position.y < playerPosition.y - verticalMaintainDistance - maintainErrorRange ||
                 (position.y > playerPosition.y && position.y < playerPosition.y + verticalMaintainDistance - maintainErrorRange)) {
            _verticalSpeed = _walkingSpeed;
        }
        else {
            _verticalSpeed = 0;
        }
        
        // Maintain horizontal distance
        if (position.x > playerPosition.x + maintainDistance + maintainErrorRange ||
            (position.x < playerPosition.x && position.x > playerPosition.x - maintainDistance + maintainErrorRange)) {
            _walkDirectionMult = -1;
        }
        else if (position.x < playerPosition.x - maintainDistance - maintainErrorRange ||
                 (position.x > playerPosition.x && position.x < playerPosition.x + maintainDistance - maintainErrorRange)) {
            _walkDirectionMult = 1;
        }
        else {
            FaceTarget();
            stopHorizontal = true;
        }

        _horizontalSpeed = _walkingSpeed * _walkDirectionMult;
        // at this point, horizontal speed is 0 (see beginning of update)
        if (AtWall() || stopHorizontal) {
            _horizontalSpeed = 0;
        }
    }

    // Move to the player
    protected override void MoveToPlayer() {
        Vector2 checkPosition = transform.position;
        Vector2 closestPoint = playerCollider.bounds.ClosestPoint(checkPosition);

        // Horizontal
        if ((transform.position.x - closestPoint.x) * controller.GetFacingMult() >= 0) {
            if (Vector2.Distance(closestPoint, checkPosition) >= minMoveToDistance) {
                if (canFlip) {
                    canFlip = false;
                    _walkDirectionMult *= -1;
                    StartCoroutine(FlipCooldown());
                }
            }
        }

        // Vertical
        if (transform.position.y - playerCollider.transform.position.y >= minMoveToDistance) {
            _verticalSpeed = -1*_walkingSpeed;
        }
        else if (transform.position.y - playerCollider.transform.position.y <= minMoveToDistance) {
            _verticalSpeed = _walkingSpeed;
        }
        else {
            _verticalSpeed = 0f;
        }
        _horizontalSpeed = _walkingSpeed * _walkDirectionMult;

        var moveVector = new Vector2(_horizontalSpeed, _verticalSpeed);
        moveVector.Normalize();
        var wallCheck = Physics2D.Raycast(transform.position, moveVector, 2f, whatIsGround);
        
        if (wallCheck.collider != null) {
            Debug.Log(wallCheck);
            _verticalSpeed = _horizontalSpeed = 0;
        }
        
        animator.SetFloat(AnimRefarences.Speed, Math.Abs(_horizontalSpeed));
    }
    
    // Move the character based on horizontal and vertical speed, flying.
    private void FixedUpdate() {
        if(_disabled) return;
        controller.FlyingMove(_horizontalSpeed,_verticalSpeed);
    }

    // Check if you passed the end of the patrol points.
    private bool AtEndOfPatrol() {
        if (transform.position.x < patrolBorderLeft && _walkDirectionMult == -1) {
            return true;
        }
        
        if (transform.position.x > patrolBorderRight && _walkDirectionMult == 1) {
            return true;
        }

        return false;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        
        Vector3 position = transform.position;
        
        // controller if not flying only if start wasnt called. This way the values are set before start.
        if (!controller.IsFlying()) {

            _moveEdgeLeftGizmo = position.x - leftPatrolDistance;
            _moveEdgeRightGizmo = position.x + rightPatrolDistance;
        }
        
        if (punch == null) return;

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
        
        
        Gizmos.DrawLine(new Vector3(_moveEdgeLeftGizmo, transform.position.y,0),
            new Vector3(_moveEdgeRightGizmo, transform.position.y,0));
        
        var v = new Vector3(_horizontalSpeed, _verticalSpeed, 0);
        v.Normalize();
        Gizmos.DrawLine(transform.position, transform.position+v*2);
    }
}
