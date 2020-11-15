using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnEnemiesDead : MonoBehaviour
{
    [SerializeField] private AllEnemiesDefeated enemies;
    // Start is called before the first frame update
    void Start()
    {
        enemies.allEnemiesDeadEvent += OnAllEnemiesDeadEvent;
    }

    private void OnAllEnemiesDeadEvent(object sender, EventArgs e) {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
