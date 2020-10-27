using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Overlay : MonoBehaviour
{
    private static UI_Overlay _uiOverlay;
    
    private void Awake() {
        if (_uiOverlay == null) {
            // DontDestroyOnLoad(gameObject);
            // _uiOverlay = this;
        }
        else {
            // Destroy(gameObject);
        }
    }
}
