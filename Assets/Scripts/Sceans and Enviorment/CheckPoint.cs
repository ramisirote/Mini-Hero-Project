using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private int checkPointValue;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            var characterStatsOnEnter = new CharacterStatsData(other.gameObject.GetComponent<CharacterStats>().GetCharacterStats());
            other.gameObject.GetComponent<PlayerDeath>().SetCheckPoint(respawnPoint, characterStatsOnEnter ,checkPointValue);
        }
    }
}
