using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITakeDamage
{ 
    void Damage(float damage, int pushDirectionMult, float pushForceUp = 70f, float pushForceSide = 70f, bool ignoreInvonerable = false);

    void Damage(float damage, Vector2 push, bool ignoreInvonerable = false);
    
    void Damage(float damage, Vector2 awayFromPos, float pushAmount, bool ignoreInvonerable = false);

    void DamageOverTime(float damage, float ticks);

    bool IsDotTicking();
}
