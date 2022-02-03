using UnityEngine;

public class Detonator : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float timer;
    [SerializeField] private float radius;
    [SerializeField] private float damage;
    [SerializeField] private float knockBack;
    [SerializeField] private Color[] colors = new Color[3];
    [SerializeField] private ParticleSystem blastParticle;
    [SerializeField] private ParticleSystem chargeParticle;

    private GameObject parent;
    private float doneTimer;
    private CharacterStats _stats;
    private bool _detonated;


    public void Init(LayerMask oEnemyLayer, float oDamage, float oTimer, float oRadius, float oKnockBack, Color[] oColors) {
        enemyLayer = oEnemyLayer;
        damage = oDamage;
        timer = oTimer;
        radius = oRadius;
        colors = oColors;
        knockBack = oKnockBack;
        
        parent = transform.parent.gameObject;
        doneTimer = timer;
        _stats = parent.GetComponent<CharacterStats>();

        SetUpParticleColors();
        chargeParticle.Play();
        var lossyScale = transform.lossyScale.y;
        Debug.Log(lossyScale);
        blastParticle.transform.localScale *= radius / lossyScale;
        chargeParticle.transform.localScale *= 1 / lossyScale;
    }
    
    private void SetUpParticleColors() {
        var col = blastParticle.colorOverLifetime;
        col.color = Utils.CreateGradient(new []{colors[0], colors[1]}, new []{0.8f, 0.0f},
            colorTimes:new []{0.35f, 0.5f});

        var chargeCol = chargeParticle.colorOverLifetime;
        chargeCol.color = Utils.CreateGradient(new []{colors[1], colors[0]},
            new []{0.5f, 0.8f},
            colorTimes:new []{0.35f, 0.5f});
    }

    private void Update() {
        if (doneTimer > 0 && !_detonated) {
            doneTimer -= Time.deltaTime;
            if (doneTimer <= 0 || (_stats && _stats.IsDead())) {
                Detonate();
            }
        }
    }

    private void Detonate() {
        var pos = transform.position;
        chargeParticle.Stop();
        blastParticle.Play();

        var hits = Physics2D.OverlapCircleAll(pos, radius, enemyLayer);

        foreach (var hit in hits) {
            var toHitVec = hit.transform.position - pos;
            toHitVec.z = 0;
            if (toHitVec.magnitude != 0) {
                toHitVec = (toHitVec / toHitVec.magnitude) * knockBack;
            }
            HitManager.GetTakeDamage(hit.gameObject)?.Damage(damage, toHitVec);
        }

        _detonated = true;
        
        Destroy(this, 1f);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
