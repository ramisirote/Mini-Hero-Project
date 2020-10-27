using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyChargeProjectile : MonoBehaviour
{
    [SerializeField] private float maxScale;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ParticleSystem explodeParticals;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TrailRenderer trailRenderer;

    private LayerMask _stopLayers;
    private float _maxCharges;
    private float _damagePerCharge;

    private Color _c1, _c2, _c3;

    private Vector2 flyDirection;
    
    private float _charges = 0;

    private float scalePerCharge;

    private bool _wasReleased = false;
    private float startTrailWidth;
    

    public void Releace(Vector2 direction, float speed) {
        flyDirection = speed * (direction / direction.magnitude);
        rb.velocity = flyDirection;
        _wasReleased = true;
    }

    public void SetUp(float maxCharges, LayerMask stopLayers, float damage, Color c1, Color c2, Color c3) {
        _maxCharges = maxCharges;
        _stopLayers = stopLayers;
        _damagePerCharge = damage;
        
        var startingScale = transform.localScale.x;
        scalePerCharge = (maxScale - startingScale) / _maxCharges;

        _c1 = c1;
        _c2 = c2;
        _c3 = c3;

        trailRenderer.colorGradient = Utils.CreateGradient(new[] {c1, c2, c3}, new[] {0.8f, 0.2f});
        startTrailWidth = trailRenderer.startWidth;
        
        SetUpColor();
    }

    private void SetUpColor() {
        var m = spriteRenderer.material;
        m.SetColor("_Color1", _c1);
        m.SetColor("_Color2", _c2);
        m.SetColor("_Color3", _c3);
    }

    public void AddCharge(float addAmount) {
        if (_charges >= _maxCharges) {
            return;
        }

        if (_charges + addAmount > _maxCharges) {
            addAmount = _maxCharges - _charges;
        }
        
        _charges += addAmount;

        var scale = transform.localScale;
        scale.x += scalePerCharge*addAmount;
        scale.y += scalePerCharge*addAmount;
        
        
        trailRenderer.startWidth = startTrailWidth*scale.x;
        
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!_wasReleased) return;
        // if other has tag that is an emeny
        if (_stopLayers==(_stopLayers | (1 << other.gameObject.layer))) {
            var damager = other.gameObject.GetComponent<TakeDamage>();
            if (damager) {
                int hitFacing = flyDirection.x > 0 ? 1 : -1;
                damager.Damage(_damagePerCharge*_charges, hitFacing);
            }
            
            
            var p = Instantiate(explodeParticals);
            p.transform.position = transform.position;
            var mainModule = p.main;
            mainModule.startColor = _c1;
            p.Play();
            
            Destroy(gameObject);
        }
    }
}
