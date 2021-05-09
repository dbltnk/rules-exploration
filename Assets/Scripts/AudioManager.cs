using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource source1;
    public AudioSource source2;
    public AudioSource source3;
    public AudioSource source4;
    public AudioSource source5;
    public AudioSource source6;

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
        if (!source4.isPlaying) source4.Play();
        if (!source5.isPlaying) source5.Play();
        if (!source6.isPlaying) source6.Play();
        ticks++;
        timeSinceLastTick = 0f;
    }

    public void Update () {
        source1.mute = false;
        source2.mute = false;
        source3.mute = (speed < 1f) ? false : true;
        source4.mute = (speed < 1f) ? false : true;
        source5.mute = (speed < 0.5f) ? false : true;
        source6.mute = (speed < 0.25f) ? false : true;

        timeSinceLastTick += Time.deltaTime;

        float volume = (fadeOutTime - Mathf.Min(fadeOutTime, timeSinceLastTick)) / fadeOutTime;

        source1.volume = volume;
        source2.volume = volume;
        source3.volume = volume;
        source4.volume = volume;
        source5.volume = volume;
        source6.volume = volume;
    }

}
