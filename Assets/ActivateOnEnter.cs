using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ActivateOnEnter : MonoBehaviour
{
    [SerializeField] private TilemapRenderer hideAreaTileMap;
    [SerializeField] private GameObject[] objectToActivate;


    // Start is called before the first frame update
    void Start()
    {
        SetActive(false);
    }

    private void SetActive(bool active) {
        if(hideAreaTileMap) hideAreaTileMap.enabled = !active;

        foreach (var ota in objectToActivate) {
            ota.SetActive(active);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
