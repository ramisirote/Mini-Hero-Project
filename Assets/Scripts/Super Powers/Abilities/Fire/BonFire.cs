using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonFire : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject onFire;
    [SerializeField] private GameObject onFireParticles;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private float dotTicks;
    [SerializeField] private float damageAmountOverTime;
    [SerializeField] private float extraDamagePerSecond;
    [SerializeField] private Material material;
    [SerializeField] private Color[] colors = new Color[3];

    private ParticleSystemRenderer particleSystemRenderer;

    private void Start() {
        SetUpColors();
    }

    public void Init(Vector3 pos, LayerMask layers, float oDotTicks, float oDamageAmountOverTime, 
                     float oExtraDamagePerSecond, Material oMaterial, Color[] oColors, float lifeTime=0) {
        transform.position = pos;
        material = oMaterial;
        layerMask = layers;
        dotTicks = oDotTicks;
        damageAmountOverTime = oDamageAmountOverTime;
        extraDamagePerSecond = oExtraDamagePerSecond;
        colors = oColors;
        SetUpColors();
        if (lifeTime!=0) {
            StartCoroutine(DelayOff(lifeTime));
        }
    }

    IEnumerator DelayOff(float lifeTime) {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    private void SetUpColors() {
        particleSystemRenderer = particles.GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.material = material;
        Utils.SetUpSpriteRenderedShaderColors(particleSystemRenderer, colors);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var otherGameObject = other.gameObject;
        if (Utils.IsObjectInLayerMask(otherGameObject, layerMask)) {
            OnFire.MakeOnFire(otherGameObject, onFireParticles, colors, true);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        var otherGameObject = other.gameObject;
        if (Utils.IsObjectInLayerMask(otherGameObject, layerMask)) {
            OnFire.MakeOnFire(otherGameObject, onFireParticles, colors, true);
        }
    }
}
