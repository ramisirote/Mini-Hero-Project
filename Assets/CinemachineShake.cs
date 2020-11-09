using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cinemachine;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera camera;

    private float _shakeTimer;
    private float _shakeTimeTotal;
    private float _startIntensity;
    
    // Start is called before the first frame update
    private void Awake() {
        Instance = this;
    }

    public void ShakeCamera(float intensity=0.5f, float time = 0.1f) {
        if (time <= 0) return;
        
        CinemachineBasicMultiChannelPerlin cinemachinePerlin =
            camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachinePerlin.m_AmplitudeGain = intensity;

        _shakeTimer = time;
        _shakeTimeTotal = time;
        _startIntensity = intensity;
    }

    private void Update() {
        if (_shakeTimer > 0) {
            _shakeTimer -= Time.deltaTime;
            CinemachineBasicMultiChannelPerlin cinemachinePerlin =
                camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachinePerlin.m_AmplitudeGain = Mathf.Lerp(_startIntensity, 0f, 1 - _shakeTimer/_shakeTimeTotal);
        }
    }
}
