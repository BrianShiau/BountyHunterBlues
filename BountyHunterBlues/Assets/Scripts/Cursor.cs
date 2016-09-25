using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour {
	public Texture2D cursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;


	// Use this for initialization
	void Start () {
		UnityEngine.Cursor.SetCursor(cursorTexture, Vector2.zero, cursorMode);
	}

	// Update is called once per frame
	void Update () {

	}
}
