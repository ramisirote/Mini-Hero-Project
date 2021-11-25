using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBolt : MonoBehaviour
{
    [SerializeField] private GameObject onFireObject;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private LayerMask layerMask;
    private float hitDamage;
    private float dotTicks;
    private float damageAmountOverTime;
    private float extraDamagePerSecond;
    private Material material;
    private Color[] colors;

    public void Init(Vector3 direction, float speed, LayerMask layers, float oHitDamage, float oDotTicks, 
                    float oDamageAmountOverTime, float oExtraDamagePerSecond, 
                    Material oMaterial, Color[] oColors) {
        if (direction == Vector3.zero) {
            direction = Vector3.forward;
        }
        direction /= ((Vector2)direction).magnitude;
        transform.eulerAngles = Vector3.forward*(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        rigidBody.velocity = direction * speed;

        material = oMaterial;
        layerMask = layers;
        hitDamage = oHitDamage;
        dotTicks = oDotTicks;
        damageAmountOverTime = oDamageAmountOverTime;
        extraDamagePerSecond = oExtraDamagePerSecond;
        colors = oColors;
        Utils.SetUpSpriteRenderedShaderColors(spriteRenderer, colors);

        material = spriteRenderer.material;
    }

    private void Update() {
        var velocity = rigidBody.velocity;
        transform.eulerAngles = Vector3.forward*(Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var go = other.gameObject;
        if (Utils.IsObjectInLayerMask(go, layerMask)) {
            OnFire.MakeOnFire(go, onFireObject, colors);
            HitManager.GetTakeDamage(go)?.Damage(hitDamage, transform.position, 10f, true);
            Destroy(gameObject);
        }
    }
}
