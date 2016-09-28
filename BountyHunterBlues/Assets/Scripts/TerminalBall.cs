using UnityEngine;
using System.Collections;

public class TerminalBall : AIActor, Interactable {
	public Door opensThisdoor;

	// Use this for initialization
	void NotCalledStart () {
	
	}
	
	// Update is called once per frame
	void NotCalledUpdate () {
	
	}

	public void runInteraction()
	{
		opensThisdoor.specialDoor = false;
		opensThisdoor.runInteraction ();
	}

	public override void updateAnimation(){
		//from GameActor.updateAnimation
		//damn this is some bad code

		// update moving state
		GameActorAnimator.SetBool("isMoving", isMoving);

		// update direction state
		if (faceDir.y != 0 && Mathf.Abs(faceDir.y) >= Mathf.Abs(faceDir.x)) // up and down facing priority over left and right
		{
			if (faceDir.y > 0)
				GameActorAnimator.SetInteger("Direction", (int)Direction.UP);
			else
				GameActorAnimator.SetInteger("Direction", (int)Direction.DOWN);
		}

		else
		{
			if (faceDir.x > 0)
				GameActorAnimator.SetInteger("Direction", (int)Direction.RIGHT);
			else
				GameActorAnimator.SetInteger("Direction", (int)Direction.LEFT);
		}

	}

	//override these so the ball does nothing
	public override void runVisionDetection(){}
	public override void attack(){}
	public override void yellow_audio(){}
	public override void yellow_alertness(){}
	public override void red_alertness(){}
	public override void return_to_default(){}
	public override void chase_alertness(){}
	public override bool sound_detection(Vector3 audio_point){return false;}
    public override void initAudio() {}
    public override void runAudio() {}
}
