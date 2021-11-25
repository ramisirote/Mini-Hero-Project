using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallProjectile : MonoBehaviour
{
    [SerializeField] private GameObject onFireObject;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private GameObject bonFire;
    [SerializeField] private float bonFireLifeTime;

    private LayerMask layerMask;
    private float hitDamage;
    private float dotTicks;
    private float damageAmountOverTime;
    private float extraDamagePerSecond;
    private Material material;
    private float radius;
    private Color[] colors;
    private ParticleSystem particleSystemInstance;

    public void Init(Vector3 direction, float speed, LayerMask layers, float oHitDamage, float oDotTicks, float oRadius,
                    float oDamageAmountOverTime, float oExtraDamagePerSecond, Material oMaterial, Color[] oColors, float oBonFireLifeTime) {
        if (direction == Vector3.zero) {
            direction = Vector3.forward;
        }
        direction /= ((Vector2)direction).magnitude;
        transform.eulerAngles = Vector3.forward*(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        rigidBody.velocity = direction * speed;

        radius = oRadius;

        material = oMaterial;
        layerMask = layers;
        hitDamage = oHitDamage;
        dotTicks = oDotTicks;
        damageAmountOverTime = oDamageAmountOverTime;
        extraDamagePerSecond = oExtraDamagePerSecond;
        colors = oColors;
        Utils.SetUpSpriteRenderedShaderColors(spriteRenderer, colors);

        material = spriteRenderer.material;

        particleSystemInstance = Instantiate(particleSystem);
        particleSystemInstance.Stop();
        particleSystemInstance.GetComponent<ParticleSystemRenderer>().material = material;

        bonFireLifeTime = oBonFireLifeTime;

    }

    private void Update() {
        var velocity = rigidBody.velocity;
        transform.eulerAngles = Vector3.forward*(Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var go = other.gameObject;
        if (Utils.IsObjectInLayerMask(go, layerMask)) {
            Explode();
            SetBonFire(other.gameObject);
        }
    }

    private void SetBonFire(GameObject go) {
        if (go.CompareTag("Ground")) {
            var bonFireGo = Instantiate(bonFire);
            var bonFireInstance = bonFireGo.GetComponent<BonFire>();
            bonFireInstance.Init(transform.position, layerMask, dotTicks, damageAmountOverTime, extraDamagePerSecond, 
                material, colors, bonFireLifeTime);
        }
    }

    private void Explode() {
        var hits = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        ExplodeParticles();
        foreach (var hit in hits) {
            OnFire.MakeOnFire(hit.gameObject, onFireObject, colors);
            HitManager.GetTakeDamage(hit.gameObject)?.Damage(hitDamage, transform.position, 10f, true);
        }
        Destroy(gameObject);
    }

    private void ExplodeParticles() {
        particleSystemInstance.transform.position = transform.position;
        particleSystemInstance.Play();
        particleSystemInstance.transform.localScale *= radius;
    }
}
