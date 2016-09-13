using UnityEngine;
using System.Collections;

public class BarrelBase : MonoBehaviour {

    public GameActor.Direction facing;

    // in parent coordinate system
    public Vector3 leftFaceLoc;
    public Vector3 rightFaceLoc;
    public Vector3 upFaceLoc;
    public Vector3 downFaceLoc;
	
	// Update is called once per frame
	void Update () {
        Vector3 newPos = transform.localPosition;
        switch (facing)
        {
            case GameActor.Direction.DOWN:
                newPos = downFaceLoc;
                break;
            case GameActor.Direction.LEFT:
                newPos = leftFaceLoc;
                break;
            case GameActor.Direction.UP:
                newPos = upFaceLoc;
                break;
            case GameActor.Direction.RIGHT:
                newPos = rightFaceLoc;
                break;

        }
        if (transform.localPosition != newPos)
            transform.localPosition = newPos;


	}
}
