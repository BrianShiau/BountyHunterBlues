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
		float distFromCenterX = Event.current.mousePosition.x - Screen.width / 2;
		float distFromCenterY = Event.current.mousePosition.y - Screen.height / 2;
		float angle = Mathf.Atan2 (distFromCenterY, distFromCenterX);
		GUIUtility.RotateAroundPivot(90+angle*Mathf.Rad2Deg, pivot);

		float x = (Event.current.mousePosition.x + Screen.width / 2) / 2 - (48 / 2);
		float y = (Event.current.mousePosition.y + Screen.height / 2) / 2 - (48 / 2);
		GUI.DrawTexture (new Rect (x, y, 48, 48), secondaryCursorTexture);		
	}
}
