using UnityEngine;
using System.Collections;

public class StatTracker : MonoBehaviour {
	private static bool created = false;
	private static int timesHit = 0;

	// Use this for initialization
	void Start () {
		if (!created) {
			DontDestroyOnLoad (transform.gameObject);
			created = true;
		} else {
			Destroy (this);
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	public static int GetTimesHit(){
		return timesHit;
	}

	public static void Hit(){
		timesHit++;
	}
}
