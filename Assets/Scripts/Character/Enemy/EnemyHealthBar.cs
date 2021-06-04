using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * Makes sure the enemy health bar doesnt flip and disappears when the enemy dies. 
 */
public class EnemyHealthBar : MonoBehaviour
{
    [FormerlySerializedAs("stats")] [SerializeField] private CharacterStats characterStats;
    [SerializeField] private GameObject baseCharacter;
    [SerializeField] private GameObject healthBarCanves;

    private float _hightOffset;

    private void Start() {
        _hightOffset = transform.position.y - baseCharacter.transform.position.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var transformLocalScale = transform.localScale;
        if (baseCharacter.transform.localScale.x >= 0 && transformLocalScale.x >= 0) {
            transformLocalScale.x *= -1;
        }
        if (baseCharacter.transform.localScale.x <= 0 && transformLocalScale.x <= 0) {
            transformLocalScale.x *= -1;
        }
        
        transform.localScale = transformLocalScale;

        if (characterStats.IsDead()) {
            healthBarCanves.SetActive(false);
        }
        
        transform.rotation = Quaternion.identity;

        transform.position = baseCharacter.transform.position + Vector3.up * _hightOffset;
    }
}
