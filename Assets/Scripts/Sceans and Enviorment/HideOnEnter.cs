using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideOnEnter : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            tilemap.color = new Color(1,1,1,0.1f);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            tilemap.color = Color.white;
        }
    }
}
