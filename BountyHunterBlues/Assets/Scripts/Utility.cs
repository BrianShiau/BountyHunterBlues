using UnityEngine;
using System.Collections;

public class Utility : MonoBehaviour {

    //  gets the vector that points from->to
	public Vector2 getDirectionVector(GameActor from, GameActor to)
    {
        Vector2 dir;
        Vector3 dir3D = to.transform.localPosition - from.transform.localPosition;
        dir = new Vector2(dir3D.x, dir3D.z);
        dir.Normalize();
        return dir;
    }
}
