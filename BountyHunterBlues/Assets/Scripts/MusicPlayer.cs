﻿using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

    public AudioClip[] songs;

    private AudioSource mainSource;
    private int currSongIndex;
    private float initialVolume;
    private static bool created = false;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(transform.gameObject);
            created = true;
        }
        else
            Destroy(this);
        
    }

	// Use this for initialization
	void Start () {
        mainSource = GetComponent<AudioSource>();
        currSongIndex = 0;
        initialVolume = mainSource.volume;
        mainSource.clip = songs[currSongIndex];
        mainSource.Play();
    }

    public void loadNextSong()
    {
        currSongIndex++;
        if (currSongIndex < songs.Length)
            StartCoroutine(fadeToNextSong());
    }

    private IEnumerator fadeToNextSong()
    {
        bool fadingOut = true;
        mainSource.volume -= .1f;
        while (mainSource.volume < initialVolume) {
            yield return StartCoroutine(Utility.WaitForRealTime(.03f));
            if(mainSource.volume <= 0)
            {
                mainSource.Stop();
                mainSource.clip = songs[currSongIndex];
                mainSource.Play();
                fadingOut = false;
            }

            if (fadingOut)
                mainSource.volume -= .1f;
            else
                mainSource.volume += .1f;
        }
    }
	
	
}
