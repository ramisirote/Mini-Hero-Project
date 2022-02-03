using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The projectile from the EnergyBoltCharge ability.
 * It cheeps charging while not released. Once released, flies in the direction, hitting the first enemy.
 * The more its charged the bigger and more damage it deals.
 */
public class EnergyChargeProjectile : MonoBehaviour
{
    [SerializeField] private float maxScale;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ParticleSystem explodeParticals;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private float baseRadius;
    [SerializeField] private float maxRadius;

    private LayerMask _stopLayers;
    private float _maxCharges;
    private float _damagePerCharge;
    private bool _isPlayer;

    private Color _c1, _c2, _c3;

    private Vector2 flyDirection;
    
    private float _charges = 0;

    private float scalePerCharge;
    private float hitRadiusPerCharge;

    private bool _wasReleased;
    private float _startTrailWidth;
    private bool _detonated;

    private float _startingScale;
    

    public void Releace(Vector2 direction, float speed) {
        gameObject.transform.SetParent(null);
        flyDirection = speed * (direction / direction.magnitude);
        rb.velocity = flyDirection;
        _wasReleased = true;
    }

    public void SetUp(float maxCharges, LayerMask stopLayers, float damage, Color c1, Color c2, Color c3, bool isPlayer) {
        _maxCharges = maxCharges;
        _stopLayers = stopLayers;
        _damagePerCharge = damage;
        _isPlayer = isPlayer;
        
        _startingScale = transform.localScale.x;
        scalePerCharge = (maxScale - _startingScale) / _maxCharges;
        hitRadiusPerCharge = (maxRadius - baseRadius) / _maxCharges;

        _c1 = c1;
        _c2 = c2;
        _c3 = c3;

        trailRenderer.colorGradient = Utils.CreateGradient(new[] {c1, c2, c3}, new[] {0.8f, 0.2f});
        _startTrailWidth = trailRenderer.startWidth;
        
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
        
        transform.localScale = scale;

        trailRenderer.startWidth = _startTrailWidth*scale.x;
    }

    public void Reset() {
        
        trailRenderer.Clear();
        _charges = 0f;
        _detonated = false;
        _wasReleased = false;
        
        transform.localScale = new Vector3(_startingScale,_startingScale, 1);

        trailRenderer.startWidth = _startTrailWidth;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!_wasReleased) return;
        // if other has tag that is an enemy
        if (Utils.IsObjectInLayerMask(other.gameObject, _stopLayers) && !_detonated) {
            _detonated = true;
            Detonate();
        }
    }

    private void Detonate() {
        var hitsDetected = 
            Physics2D.OverlapCircleAll(transform.position, baseRadius + _charges*hitRadiusPerCharge, _stopLayers);
        foreach (var hit in hitsDetected) {
            var damager = HitManager.GetTakeDamage(hit.gameObject);
            if (damager != null) {
                int hitFacing = flyDirection.x > 0 ? 1 : -1;
                damager.Damage(_damagePerCharge*_charges, hitFacing);
            }
        }

        var p = Instantiate(explodeParticals);
        p.transform.localScale *= Math.Max(0.2f,_charges/_maxCharges);
        p.transform.position = transform.position;
        var mainModule = p.main;
        mainModule.startColor = _c1;
        p.Clear();
        p.Play();
        
        if(_isPlayer) CinemachineShake.Instance.ShakeCamera(2f*GetChargeNormalized());
        
        gameObject.SetActive(false);
    }

    public float GetChargeNormalized() {
        return _charges / _maxCharges;
    }
    
    void OnDrawGizmosSelected(){
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, baseRadius);
        
        Gizmos.color = new Color(_charges/_maxCharges, 0, 0);
        Gizmos.DrawWireSphere(transform.position, baseRadius + _charges*hitRadiusPerCharge);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }

    private void OnDisable() {
        Reset();
    }
}
