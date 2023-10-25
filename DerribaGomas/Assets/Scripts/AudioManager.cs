using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public static void Play(string audio)
    {
        AudioClip clip = (AudioClip) Resources.Load("Audio/" + audio);
        source.PlayOneShot(clip);
    }
}
