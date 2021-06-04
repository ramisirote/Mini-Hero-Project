using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Overlay : MonoBehaviour
{
    public static UI_Overlay uiOverlay;

    [SerializeField] private RawImage fadeImage;
    [SerializeField] private float fadeTimeIn;
    [SerializeField] private float fadeTimeOut;
    
    private Color fadedOutColor = Color.clear;
    private Color fadedInColor = Color.black;
    private float fadeTimer;
    
    private void Awake() {
        if (uiOverlay == null) {
            uiOverlay = this;
        }
    }

    public void Fade() {
        StartCoroutine(FadeInOut());
    }

    IEnumerator FadeInOut() {
        fadeTimer = fadeTimeIn;
        while (fadeTimer >= 0) {
        
            fadeImage.color = Color.Lerp(fadedOutColor, fadedInColor, 1-fadeTimer/fadeTimeIn);
            
            fadeTimer -= Time.deltaTime;
            yield return null;
        }
        
        fadeImage.color = fadedInColor;
        
        fadeTimer = fadeTimeOut;
        while (fadeTimer >= 0) {

            fadeImage.color = Color.Lerp(fadedInColor, fadedOutColor, 1-fadeTimer/fadeTimeOut);
            
            fadeTimer -= Time.deltaTime;
            yield return null;
        }

        fadeImage.color = fadedOutColor;
    }
}
