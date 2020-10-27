using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableAddHp : MonoBehaviour
{
    [SerializeField] private int hpWorth;
    [SerializeField] private ParticleSystem particleSys;
    [SerializeField] private AudioSource audioSource;
    


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            var stats = other.GetComponent<CharacterStats>().GetCharacterStats();
            if (stats.Health >= stats.MaxHealth) {
                return;
            }
            stats.ChangeHpBy(hpWorth);
            particleSys.Play();
            audioSource.PlayOneShot(audioSource.clip);
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(gameObject, particleSys.main.startLifetime.constant);
        }
    }
}
