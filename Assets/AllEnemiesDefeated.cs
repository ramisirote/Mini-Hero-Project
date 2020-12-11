using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllEnemiesDefeated : MonoBehaviour
{
    public event EventHandler allEnemiesDeadEvent;
    private TakeDamage[] enemyDamages;

    private int numberOfEnemies;

    private int numberOfEnemiesDead;
    
    // Start is called before the first frame update
    void Start() {
        enemyDamages = GetComponentsInChildren<TakeDamage>();
        numberOfEnemies = enemyDamages.Length;
        foreach (var enemyDamage in enemyDamages) {
            enemyDamage.OnDeathEvent += EnemyOnDeathEvent;
        }
    }

    private void EnemyOnDeathEvent(object sender, IManager manager) {
        numberOfEnemiesDead++;
        if (numberOfEnemies == numberOfEnemiesDead) {
            allEnemiesDeadEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
