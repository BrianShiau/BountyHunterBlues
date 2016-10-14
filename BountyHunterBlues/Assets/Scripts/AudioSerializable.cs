using UnityEngine;


public class NamedAudioSource
{
    public string name;
    public AudioSource source;
}

[System.Serializable]
public class NamedAudioClip
{
    public string name;
    public AudioClip clip;
}

[System.Serializable]
public class AudioSerializable
{
    public string sourceName;
    public NamedAudioClip[] Clips;
}

