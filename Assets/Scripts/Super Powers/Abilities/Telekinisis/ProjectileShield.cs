using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShield : MonoBehaviour
{
    private Collider2D mCollider;
    public void Init(LayerMask layer, Vector3 position, Vector3 rotation) {
        gameObject.layer = (int) Mathf.Log(layer.value, 2);
        var mTransform = transform;
        mTransform.position = new Vector3(position.x, position.y, 0);
        mTransform.eulerAngles = rotation;
    }
    
    public void UpdateLocation(Vector3 position, Vector3 rotation) {
        var mTransform = transform;
        mTransform.position = new Vector3(position.x, position.y, 0);
        mTransform.eulerAngles = rotation;
    }

    public void Disable() {
        Destroy(gameObject);
    }

    private void Start() {
        mCollider = gameObject.GetComponent<Collider2D>();
    }
}
