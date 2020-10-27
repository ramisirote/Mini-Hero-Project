using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource defaultSource;
    [SerializeField] private SoundSource[] sounds;
    
    
    [System.Serializable]
    public enum SoundClips
    {
        Walk, LevelUp, TakeDamage, Jump, Die, EnemyDetected
    }


    private void Awake() {
        foreach (var sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.spatialBlend = sound.spacialBlend;
            sound.source.maxDistance = defaultSource.maxDistance;
            sound.source.rolloffMode = defaultSource.rolloffMode;
            sound.source.clip = sound.audioClip;
            sound.source.playOnAwake = false;
        }
    }


    public void PlayAudio(SoundClips clip) {
        foreach (var sound in sounds) {
            if (sound.soundClip != clip) continue;
            
            if (!sound.loop || !sound.source.isPlaying) {
                sound.source.Play();
            }
        }
    }
    
    public void StopAudio(SoundClips clip) {
        foreach (var sound in sounds) {
            if (sound.soundClip == clip) {
                sound.source.Stop();
            }
        }
    }

    private void PlayOneShot(AudioClip audioClip) {
        defaultSource.PlayOneShot(audioClip);
    }

    [System.Serializable]
    public class SoundSource
    {
        public SoundClips soundClip;
        public AudioClip audioClip;
        [Range(0,1)] public float volume = 1;

        [Range(-3,3)] public float pitch;

        public bool loop;

        [Range(0,1)] public float spacialBlend = 1;

        [HideInInspector] public AudioSource source;
    }
}


