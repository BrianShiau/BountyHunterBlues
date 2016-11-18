using UnityEngine;
using System.Collections;

public class audio_RunningScript : MonoBehaviour {
    /*
	public AudioClip[] audioSources = new AudioClip[5];
	public AudioSource audio;
	private float baseFootAudioVolume = 1.0f;
	private float soundEffectPitchRandomness = 0.05f;

	// Use this for initialization
	void Start () {
		audio = this.GetComponent<AudioSource>;
	}
	
	// Update is called once per frame
	void Update () {
		bool movement = false;
		AudioClip nextClip = audioSources[Random.Range(0, audioSources.Length)];
		audio.volume = 1.0;

		if(player.InTacticalMode() || player.InDialogueMode())
		{
			
		}
		else
		{ 
			
			// basing WASD on +x-axis, +y-axis, -x-axis, -y-axis respectively
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
			{
				movement = true;
			}
			
			if (Input.GetKey(KeyCode.A))
			{
				
				movement = true;
			}
			if (Input.GetKey(KeyCode.S))
			{
				
				movement = true;
			}
			if (Input.GetKey(KeyCode.D))
			{
				
				//movement = true;
			//}
			
			
			if (movement)
			{
				audio.clip = nextClip;
				audio.volume = audio.volume * baseFootAudioVolume;
				audio.pitch = Random.Range(1.0 - soundEffectPitchRandomness, 1.0 + soundEffectPitchRandomness);
				audio.Play();
			}
			else
				// issue stopMove command if no WASD input given this frame
				nextCommands.AddLast(stopMove);
		}
	}
    */
}
