using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyHealthBar : MonoBehaviour
{
    [FormerlySerializedAs("stats")] [SerializeField] private CharacterStats characterStats;
    [SerializeField] private GameObject baseCharacter;
    [SerializeField] private GameObject healthBarCanves;

    // Update is called once per frame
    void Update()
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
    }
}
