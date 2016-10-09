using UnityEngine;

[System.Serializable]
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
    public NamedAudioSource Source;
    public NamedAudioClip[] Clips;
}

