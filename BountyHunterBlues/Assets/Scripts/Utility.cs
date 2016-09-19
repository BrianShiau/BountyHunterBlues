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

	// yields for the real time delay even when timescale is set to 0 and the game is paused
	public static IEnumerator WaitForRealTime(float delay){
		while(true){
			float pauseEndTime = Time.realtimeSinceStartup + delay;
			while (Time.realtimeSinceStartup < pauseEndTime){
				Debug.Log (Time.realtimeSinceStartup);
				yield return 0;
			}
			break;
		}
	}
}
