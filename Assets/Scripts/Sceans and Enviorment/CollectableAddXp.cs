using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableAddXp : MonoBehaviour
{
    [SerializeField] private int xpWorth;
    [SerializeField] private ParticleSystem particleSys;
    [SerializeField] private AudioSource audioSource;


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<CharacterStats>().GetCharacterStats().AddXp(xpWorth);
            particleSys.Play();
            audioSource.Play();
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(gameObject, particleSys.main.startLifetime.constant);
        }
    }
}
