using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour, Interactable {

	public bool currentlyTalking;

	// Use this for initialization
	void Start () {
		currentlyTalking = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void runInteraction()
	{
		if (!currentlyTalking) {
			
		}
	}
}
