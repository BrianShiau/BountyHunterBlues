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
				yield return 0;
			}
			break;
		}
	}

	// draws a shitty line
	public static IEnumerator drawLine(Vector3 start , Vector3 end, Color color,float duration = 0.2f){
		GameObject myLine = new GameObject ();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer> ();
		LineRenderer lr = myLine.GetComponent<LineRenderer> ();
		lr.material = new Material (Shader.Find ("Particles/Additive"));
		lr.SetColors (color,color);
		lr.SetWidth (0.1f,0.1f);
		lr.SetPosition (0, start);
		lr.SetPosition (1, end);
		lr.sortingOrder = 100;
		yield return new WaitForSeconds(duration);
		GameObject.Destroy (myLine);
	}
}
