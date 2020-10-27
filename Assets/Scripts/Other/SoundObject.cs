using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundObject
{
    public AudioClip audioClip;
    public float volume;

    public float pitch;

    public bool loop;

    [HideInInspector] public AudioSource source;
}
