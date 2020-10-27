using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Turn invisible for a short time.
 * Enemies can't see you while invisible (but can remember your position for a bit)
 */
public class Invisibility : Ability
{
    // Start is called before the first frame update
    [SerializeField] private float powerDuration;
    [SerializeField][Range(0, 1)] private float invisibilityAlpha;
    [SerializeField] private LayerMask invisibleLayer;
    
    private SpriteHandler _spriteHandler;
    private int defaultLayerValue;
    private int _invisibleLayerNumber;

    protected override void AdditionalInit() {
        _spriteHandler = parentCharacter.GetComponent<SpriteHandler>();
        // if(_stats)
        //     _stats.GetCharacterStats().OnDamageTaken += OnTakeDamage;
        _invisibleLayerNumber = (int) Mathf.Log(invisibleLayer.value, 2);
    }

    protected override void OnDamageTaken(object o, float damageTaken) {
        // if(AbilityOn) SetAbilityOff();
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn) {
            _spriteHandler.ColorizeAllSprites(new Color(1,1,1, invisibilityAlpha));
            defaultLayerValue = parentCharacter.layer;
            
            parentCharacter.layer = _invisibleLayerNumber;

            NextCanUse = Time.time + (abilityCooldown + powerDuration) * Time.deltaTime;

            StartCoroutine(TurnOffAfterDuration());
        }
        else {
            // SetPowerOff();
        }
    }

    private IEnumerator TurnOffAfterDuration() {
        AbilityOn = true;
        AbilityOnInvoke();
        
        yield return new WaitForSeconds(powerDuration);

        if(AbilityOn) SetAbilityOff();
    }

    public override void SetAbilityOff() {
        if (!parentCharacter || !AbilityOn) {
            AbilityOn = false;
            return;
        }
        
        
        parentCharacter.layer = defaultLayerValue;

        _spriteHandler.ColorizeAllSprites(Color.white);
        
        NextCanUse = Time.time + abilityCooldown * Time.deltaTime;
        
        AbilityOffInvoke();
        AbilityOn = false;
    }
}
