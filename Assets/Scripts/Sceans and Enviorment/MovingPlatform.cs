using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float leftMoveDistance;
    [SerializeField] private float rightMoveDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool startRight = true;

    private Vector3 _velocity;
    private float _smoothing = 0.3f;

    private Vector3 _moveEdgeLeft;
    private Vector3 _moveEdgeRight;

    private Vector3 _targetPosition;

    private Dictionary<String,Transform> _parentsTransforms = new Dictionary<string, Transform>();
    private Transform _otherParent;
    
    // Start is called before the first frame update
    void Start() {

        Vector3 position = transform.position;

        _moveEdgeLeft = new Vector3(position.x - leftMoveDistance, position.y, 0f);
        _moveEdgeRight = new Vector3(position.x + rightMoveDistance, position.y, 0f);

        if (startRight) {
            _targetPosition = _moveEdgeRight;
        }
        else {
            _targetPosition = _moveEdgeLeft;
        }
    }

    // Update is called once per frame
    void Update() {
        float xPosition = transform.position.x;
        if (xPosition >= _moveEdgeRight.x && _targetPosition == _moveEdgeRight) {
            _targetPosition = _moveEdgeLeft;
        }
        else if (xPosition <= _moveEdgeLeft.x && _targetPosition == _moveEdgeLeft) {
            _targetPosition = _moveEdgeRight;
        }
    }

    void FixedUpdate() {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition,
                                            moveSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.transform.parent == transform) return;
        if (!other.gameObject.CompareTag("Player") 
            && !other.gameObject.CompareTag("Enemy") 
            && !other.gameObject.CompareTag("Collectable")) return;
        
        var controller = other.gameObject.GetComponent<CharacterController2D>();
                
        _parentsTransforms[other.gameObject.name] = other.transform.parent;
                
        if (controller != null && !controller.IsGrounded()) return;
        
        other.transform.parent = transform;
    }

    
    private void OnCollisionStay2D(Collision2D other) {
        if (other.transform.parent == transform) return;
        if (!other.gameObject.CompareTag("Player") 
            && !other.gameObject.CompareTag("Enemy") 
            && !other.gameObject.CompareTag("Collectable")) return;
        
        var controller = other.gameObject.GetComponent<CharacterController2D>();
                
        _parentsTransforms[other.gameObject.name] = other.transform.parent;
                
        if (controller != null && !controller.IsGrounded()) return;
        
        other.transform.parent = transform;
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Collectable")) {
            
            other.transform.parent = _parentsTransforms[other.gameObject.name];
        }
    }

    void OnDrawGizmosSelected() {
        float moveEdgeLeftGizmo = 0;
        float moveEdgeRightGizmo = 0;
        if (_targetPosition == Vector3.zero) {
            Vector3 position = transform.position;

            moveEdgeLeftGizmo = position.x - leftMoveDistance;
            moveEdgeRightGizmo = position.x + rightMoveDistance;
        }
        
        
        Gizmos.DrawLine(new Vector3(moveEdgeLeftGizmo, transform.position.y,0),
            new Vector3(moveEdgeRightGizmo, transform.position.y,0));
    }
}
