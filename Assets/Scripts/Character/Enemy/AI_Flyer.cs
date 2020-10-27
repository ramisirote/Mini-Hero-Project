using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = System.Random;

/*
 * 
 */
public class AI_Flyer : AIBase
{
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
        
        // controller if not flying only if start wasnt called. This way the values are set before start.
        if (!controller.isFlying()) {
            Vector3 position = transform.position;

            _moveEdgeLeftGizmo = position.x - leftPatrolDistance;
            _moveEdgeRightGizmo = position.x + rightPatrolDistance;
        }
        
        
        Gizmos.DrawLine(new Vector3(_moveEdgeLeftGizmo, transform.position.y,0),
            new Vector3(_moveEdgeRightGizmo, transform.position.y,0));
    }
}
