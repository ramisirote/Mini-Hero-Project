using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        gameObject.GetComponent<TakeDamage>().OnDeathEvent += (sender, args) => {
            StartCoroutine(FadeAwayDisable());
        };
    }

    private IEnumerator FadeAwayDisable() {
        yield return new WaitForSeconds(5);
        gameObject.SetActive(false);
    }

    
}
