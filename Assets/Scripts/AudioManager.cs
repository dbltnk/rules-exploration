using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource source1;
    public AudioSource source2;
    public AudioSource source3;

    public AudioClip A1;
    public AudioClip A2;
    public AudioClip A4;
    public AudioClip A8;
    public AudioClip A16;
    public AudioClip MXSynth;
    public AudioClip MXBeat;
    public AudioClip MXArp;

    private int ticks = 0;
    public float timeSinceLastTick = 0f;
    public float fadeOutTime = 2f;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Tick(float updateRate) {
        speed = updateRate;
        if (!source1.isPlaying) source1.Play();
        if (!source2.isPlaying) source2.Play();
        if (!source3.isPlaying) source3.Play();

        //if (updateRate >= 0.5f) source.PlayOneShot(A2);
        //if (updateRate >= 0.25f) source.PlayOneShot(A4);
        //if (updateRate >= 0.125f) source.PlayOneShot(A8);
        //if (updateRate >= 0.06125f) source.PlayOneShot(A16);
        ticks++;
        timeSinceLastTick = 0f;
    }

    public void Update () {
        source2.mute = (speed < 1f) ? false : true;
        source3.mute = (speed < 0.5f) ? false : true;

        //timeSinceLastTick += Time.deltaTime;

        //float volume = (fadeOutTime - Mathf.Min(fadeOutTime, timeSinceLastTick)) / fadeOutTime;

        //source1.volume = volume;
        //source2.volume = volume;
        //source3.volume = volume;
    }

}
