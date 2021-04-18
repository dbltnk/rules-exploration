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

    // Start is called before the first frame update
    void Start()
    {
        source1.loop = true;
        source2.loop = true;
        source3.loop = true;
    }

    public void Tick(float updateRate) {
        if (!source1.isPlaying) source1.PlayOneShot(MXSynth);
        if (!source2.isPlaying) source2.PlayOneShot(MXBeat);
        if (!source3.isPlaying) source3.PlayOneShot(MXArp);

        source2.mute = (updateRate < 1f) ? false : true;
        source3.mute = (updateRate < 0.5f) ? false : true;

        //if (updateRate >= 0.5f) source.PlayOneShot(A2);
        //if (updateRate >= 0.25f) source.PlayOneShot(A4);
        //if (updateRate >= 0.125f) source.PlayOneShot(A8);
        //if (updateRate >= 0.06125f) source.PlayOneShot(A16);
        ticks++;
        timeSinceLastTick = 0f;
    }

    public void Update () {
        timeSinceLastTick += Time.deltaTime;

        float volume = (fadeOutTime - Mathf.Min(fadeOutTime, timeSinceLastTick)) / fadeOutTime;

        source1.volume = volume;
        source2.volume = volume;
        source3.volume = volume;
    }

}
