using UnityEngine;
using System.Collections;

public class TerminalBall : NPCActor, Interactable {
	public Door opensThisdoor;

    private bool interacted = false;

	// Use this for initialization
	public override void Start(){
        base.Start();
	}
	
	// Update is called once per frame
	public override void Update(){
        base.Update();
	}

	public void runInteraction()
	{
		interacted = true;
		opensThisdoor.specialDoor = false;
		opensThisdoor.runInteraction ();
        audioManager.Play("Beep");
	}
}
