using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour {
	public Texture2D cursorTexture;
	public Texture2D secondaryCursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;


	// Use this for initialization
	void Start () {
		UnityEngine.Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.height/2, cursorTexture.width/2), cursorMode);
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){
		Vector2 pivot = new Vector2(Screen.width/2, Screen.height/2);
		GUIUtility.RotateAroundPivot(45%360,pivot);
		GUI.DrawTexture (new Rect (
			(Event.current.mousePosition.x + Screen.width / 2) / 2 - (32 / 2),
			(Event.current.mousePosition.y + Screen.height / 2) / 2 - (32 / 2),
			32, 32),
			secondaryCursorTexture);		
	}
}
