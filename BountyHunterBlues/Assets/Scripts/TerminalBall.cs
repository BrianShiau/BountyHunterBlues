using UnityEngine;
using System.Collections;

public class TerminalBall : NPCActor, Interactable {
	public Door opensThisdoor;

    // Audio
    public AudioClip beepSound;

    private AudioSource beepSoundSource;
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
	}

	public override void runAnimation(){
		//from GameActor.updateAnimation
		//damn this is some bad code

		// update moving state
		gameActorAnimator.SetBool("isMoving", isMoving);

		// update direction state
		if (faceDir.y != 0 && Mathf.Abs(faceDir.y) >= Mathf.Abs(faceDir.x)) // up and down facing priority over left and right
		{
			if (faceDir.y > 0)
				gameActorAnimator.SetInteger("Direction", (int)Direction.UP);
			else
				gameActorAnimator.SetInteger("Direction", (int)Direction.DOWN);
		}

		else
		{
			if (faceDir.x > 0)
				gameActorAnimator.SetInteger("Direction", (int)Direction.RIGHT);
			else
				gameActorAnimator.SetInteger("Direction", (int)Direction.LEFT);
		}

	}
}
