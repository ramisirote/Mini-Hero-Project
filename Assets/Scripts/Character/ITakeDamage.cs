using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITakeDamage
{ 
    void Damage(float damage, int pushDirectionMult, float pushForceUp = 70f, float pushForceSide = 70f);

    void Damage(float damage, Vector2 push);

    void DamageOverTime(float damage, float ticks);

    bool IsDotTicking();
}
