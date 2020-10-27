using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    private float _offsetX;
    private float _offsetY;
    private float _yOffsetThreshold;
    
    // Start is called before the first frame update
    void Start() {
        _offsetX = transform.position.x - player.position.x;
        _offsetY = transform.position.y - player.position.y;
    }

    private void LateUpdate() {
        
        var position = transform.position;
        var playerPosition = player.position;

        position.x = playerPosition.x + _offsetX;
        position.y = playerPosition.y + _offsetY;
        
        transform.position = position;
    }
}
